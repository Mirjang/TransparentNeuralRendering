import os.path
import random
import torchvision.transforms as transforms
import torch
import numpy as np
from data.base_dataset import BaseDataset
#from data.image_folder import make_dataset
from PIL import Image
import OpenEXR
import glob
from util.exr import channels_to_ndarray

#for loading binary data
# def make_dataset(dir):
#     rgb = []
#     uv = [] 
#     mask = []
#     assert os.path.isdir(dir), '%s is not a valid directory' % dir
#     for root, _, fnames in sorted(os.walk(dir)):
#         for fname in fnames:
#             if any(fname.endswith(extension) for extension in ['rgb.bin', 'rgb.BIN']):
#                 path = os.path.join(root, fname)
#                 rgb.append(path)
#             elif any(fname.endswith(extension) for extension in ['uv.bin', 'uv.BIN']):
#                 path = os.path.join(root, fname)
#                 uv.append(path)
#             elif any(fname.endswith(extension) for extension in ['mask.bin', 'mask.BIN']):
#                 path = os.path.join(root, fname)
#                 mask.append(path)
#     paths = zip(rgb,uv,mask)
#     return paths

def make_dataset(dir, opt):
    paths = [] 

    assert os.path.isdir(dir), '%s is not a valid directory' % dir
    i = 0
    while os.path.exists(os.path.join(dir, str(i) + "_rgb.png")):
        rgb = os.path.join(dir, str(i) + "_rgb.png")
        uvs = [] 
        for l in range(opt.num_depth_layers):
            uvs.append(os.path.join(dir, str(i) + "_uv_"+str(l)+".png"))
        #too slow for large datasets
        #uvs = sorted(glob.glob(os.path.join(dir, str(i) + "_uv_*.exr")))
        paths.append((rgb, sorted(uvs)))
        i = i+1

    return paths

class TransparentPNGDataset(BaseDataset):
    @staticmethod
    def modify_commandline_options(parser, is_train):
        return parser

    def initialize(self, opt):
        self.opt = opt
        self.root = opt.dataroot
        self.dir_AB = os.path.join(opt.dataroot, opt.phase)
        self.AB_paths = sorted(make_dataset(self.dir_AB, opt))
        assert(opt.resize_or_crop == 'resize_and_crop')
        self.extrinsics = np.loadtxt(os.path.join(self.dir_AB, "camera_pose.txt"))
        self.IMG_DIM_X = 512
        self.IMG_DIM_Y = 512
        #self.device = torch.device('cuda:{}'.format(opt.gpu_ids[0])) if opt.gpu_ids else torch.device('cpu')
        self.device = torch.device('cpu')
        print("DataLoader using: " + str(self.device))

    def __getitem__(self, index):
        #print('GET ITEM: ', index)
        AB_path = self.AB_paths[index]

        rgb_path, uv_paths = AB_path

        assert(len(uv_paths) >= self.opt.num_depth_layers), "len(uv_paths) !>= num_depth_layers"
        # default image dimensions

        # load image data
        #assert(IMG_DIM == self.opt.fineSize)
        
        rgb_array = Image.open(rgb_path)

        uv_arrays = []
        mask_arrays = [] 

        for i in range(self.opt.num_depth_layers):
            uvm = transforms.ToTensor()(Image.open(uv_paths[i])).to(self.device)
            mask_tmp = uvm[2:3]
            uv = uvm[:2]
            mask_tmp = mask_tmp * 255
            mask_tmp[mask_tmp == 255] = 0
            mask_arrays.append(mask_tmp.round().int())
            uv_mask = torch.cat([mask_tmp, mask_tmp], 0)
            uv[uv_mask == 0] = 0 #rendering forces background to be 1, however here 0 is preferable
            uv_arrays.append(uv)

        UV = torch.cat(uv_arrays, 0)
        MASK = torch.cat(mask_arrays, 0)

        TARGET = transforms.ToTensor()(rgb_array)

        # print(UV.shape)
        # print(MASK.shape)
        # print(TARGET.shape)

        # UV = transforms.ToTensor()(uvs.astype(np.float32))
        # MASK = transforms.ToTensor()(masks.astype(np.int32))

        # for i in range(self.opt.num_depth_layers):
        #     uvm = np.array(Image.open(uv_paths[i]))
        #     mask_tmp = uvm[:,:, 2:3]
        #     uv = uvm[:,:,:2]       
        #     mask_tmp = mask_tmp * 255
        #     mask_tmp[mask_tmp==255] = 0
        #     mask_arrays.append( np.rint(mask_tmp).astype(np.int32))
        #     uv_mask = np.concatenate([mask_tmp,mask_tmp], axis=2)
        #     uv[uv_mask==0] = 0 #rendering forces background to be 1, however here 0 is preferable
        #     uv_arrays.append( uv )

        # uvs = np.concatenate(uv_arrays, axis=2)
        # masks = np.concatenate(mask_arrays, axis=2)

        # TARGET = transforms.ToTensor()(rgb_array)
        # UV = transforms.ToTensor()(uvs.astype(np.float32))
        # MASK = transforms.ToTensor()(masks.astype(np.int32))

        TARGET = 2.0 * TARGET - 1.0
        UV = 2.0 * UV - 1.0
        # print(UV.shape)
        # print(MASK.shape)
        # print(TARGET.shape)

        #################################
        ####### apply augmentation ######
        #################################
        # if not self.opt.no_augmentation:
        #     # random dimensions
        #     new_dim_x = np.random.randint(int(IMG_DIM_X * 0.75), IMG_DIM_X+1)
        #     new_dim_y = np.random.randint(int(IMG_DIM_Y * 0.75), IMG_DIM_Y+1)
        #     new_dim_x = int(np.floor(new_dim_x / 64.0) * 64 ) # << dependent on the network structure !! 64 => 6 layers
        #     new_dim_y = int(np.floor(new_dim_y / 64.0) * 64 )
        #     if new_dim_x > IMG_DIM_X: new_dim_x -= 64
        #     if new_dim_y > IMG_DIM_Y: new_dim_y -= 64

        #     # random pos
        #     if IMG_DIM_X == new_dim_x: offset_x = 0
        #     else: offset_x = np.random.randint(0, IMG_DIM_X-new_dim_x)
        #     if IMG_DIM_Y == new_dim_y: offset_y = 0
        #     else: offset_y = np.random.randint(0, IMG_DIM_Y-new_dim_y)

        #     # select subwindow
        #     TARGET = TARGET[:, offset_y:offset_y+new_dim_y, offset_x:offset_x+new_dim_x]
        #     UV = UV[:, offset_y:offset_y+new_dim_y, offset_x:offset_x+new_dim_x]



        # else:
        #     new_dim_x = int(np.floor(IMG_DIM_X / 64.0) * 64 ) # << dependent on the network structure !! 64 => 6 layers
        #     new_dim_y = int(np.floor(IMG_DIM_Y / 64.0) * 64 )
        #     offset_x = 0
        #     offset_y = 0
        #     # select subwindow
        #     TARGET = TARGET[:, offset_y:offset_y+new_dim_y, offset_x:offset_x+new_dim_x]
        #     UV = UV[:, offset_y:offset_y+new_dim_y, offset_x:offset_x+new_dim_x]


        #################################

        extrinsics = torch.tensor(self.extrinsics[index].astype(np.float32))
        return {'TARGET': TARGET, 'UV': UV, 'MASK' : MASK,
                'paths': rgb_path, 'extrinsics' : extrinsics}

    def __len__(self):
        return len(self.AB_paths)

    def name(self):
        return 'TransparentPNGDataset'