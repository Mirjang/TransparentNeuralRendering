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

def make_dataset(dir):
    paths = [] 

    assert os.path.isdir(dir), '%s is not a valid directory' % dir
    i = 0   
    while os.path.exists(os.path.join(dir, str(i) + "_rgb.exr")): 
        rgb = os.path.join(dir, str(i) + "_rgb.exr")
        uvs = sorted(glob.glob(os.path.join(dir, str(i) + "_uv_*.exr")))
        paths.append((rgb,uvs))
        i = i+1

    return paths

def load_intrinsics(input_dir):
    file = open(input_dir+"/intrinsics.txt", "r")
    intrinsics = [[(float(x) for x in line.split())] for line in file]
    file.close()
    intrinsics = list(intrinsics[0][0])
    return intrinsics

def load_rigids(input_dir):
    file = open(input_dir+"/rigid.txt", "r")
    rigid_floats = [[float(x) for x in line.split()] for line in file] # note that it stores 5 lines per matrix (blank line)
    file.close()
    all_rigids = [ [rigid_floats[4*idx + 0],rigid_floats[4*idx + 1],rigid_floats[4*idx + 2],rigid_floats[4*idx + 3]] for idx in range(0, len(rigid_floats)//4) ]
    return all_rigids


def loadRGBAFloatEXR(path, channel_names = ['R', 'G', 'B']): 
    assert(OpenEXR.isOpenExrFile(path))

    exr_file = OpenEXR.InputFile(path)
    nparr = channels_to_ndarray(exr_file, channel_names)
    exr_file.close()
    nparr = np.clip(nparr, 0.0, 1.0)
    
    #rgb = np.transpose(rgb, (1,2,0))
    #rgb = rgb[:,:, :3]
    #rgb = np.flip(rgb, 0)

    return nparr


class TransparentDataset(BaseDataset):
    @staticmethod
    def modify_commandline_options(parser, is_train):
        return parser

    def initialize(self, opt):
        self.opt = opt
        self.root = opt.dataroot
        self.dir_AB = os.path.join(opt.dataroot, opt.phase)
        self.AB_paths = sorted(make_dataset(self.dir_AB))
        assert(opt.resize_or_crop == 'resize_and_crop')


        self.IMG_DIM_X = 512
        self.IMG_DIM_Y = 512

    def __getitem__(self, index):
        #print('GET ITEM: ', index)
        AB_path = self.AB_paths[index]

        rgb_path, uv_paths = AB_path

        assert(len(uv_paths) == self.opt.num_depth_layers), "len(uv_paths) != num_depth_layers"
        # default image dimensions


        # load image data
        #assert(IMG_DIM == self.opt.fineSize)
        rgb_array = loadRGBAFloatEXR(rgb_path,['R', 'G', 'B'])

        uv_arrays = []
        mask_arrays = [] 

        for i in range(self.opt.num_depth_layers):
            mask_tmp = loadRGBAFloatEXR(uv_paths[i],channel_names=['B'])
            mask_tmp = mask_tmp * 255
            mask_tmp[mask_tmp==255] = 0
            mask_arrays.append( np.rint(mask_tmp).astype(np.int32))
            uv = loadRGBAFloatEXR(uv_paths[i], channel_names=['R','G'])
            uv_mask = np.concatenate([mask_tmp,mask_tmp], axis=2)
            uv[uv_mask==0] = 0 #rendering forces background to be 1, however here 0 is preferable
            uv_arrays.append( uv )

        uvs = np.concatenate(uv_arrays, axis=2)
        masks = np.concatenate(mask_arrays, axis=2)

        TARGET = transforms.ToTensor()(rgb_array.astype(np.float32))
        UV = transforms.ToTensor()(uvs.astype(np.float32))
        MASK = transforms.ToTensor()(masks.astype(np.int32))

        #debug
        # UV = transforms.ToTensor()(uv_arrays[0].astype(np.float32))
        # MASK = transforms.ToTensor()(mask_arrays[0].astype(np.int32))

        TARGET = 2.0 * TARGET - 1.0
        UV = 2.0 * UV - 1.0


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

        return {'TARGET': TARGET, 'UV': UV, 'MASK' : MASK,
                'paths': AB_path,}

    def __len__(self):
        return len(self.AB_paths)

    def name(self):
        return 'TransparentDataset'
