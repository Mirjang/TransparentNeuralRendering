import time
from options.train_options import TrainOptions
from options.test_options import TestOptions

from data import CreateDataLoader, transparent_dataset
from models import create_model
from util.visualizer import Visualizer
from util.util import imshow
import numpy as np
import torch
import os
import glob
import matplotlib.pyplot as plt

if __name__ == '__main__':
    opt = TrainOptions().parse()
    if opt.id_mapping: 
        id_mapping = list(opt.id_mapping.split(","))
        opt.id_mapping = list(map(int, id_mapping)) 
        print("using mapping: " + str(opt.id_mapping))

    # hard-code some parameters for test
    opt.num_threads = 1   # test code only supports num_threads = 1
    opt.batch_size = 1    # test code only supports batch_size = 1
    opt.serial_batches = True  # no shuffle
    opt.no_augmentation = True    # no flip
    opt.display_id = -1   # no visdom display
    opt.num_threads = 0
    data_loader = CreateDataLoader(opt)
    dataset = data_loader.load_data()
    dataset_size = len(data_loader)
    print('#test images = %d' % dataset_size)
    print('#test objects = %d' % opt.nObjects)

    opt = TestOptions().parse()
    opt.phase="test"
    opt.serial_batches = True  # no shuffle

    #opt.id_mapping = "0,2,3,4,5,-1"
    if opt.id_mapping: 
        id_mapping = list(opt.id_mapping.split(","))
        opt.id_mapping = list(map(int, id_mapping)) 
        print("using mapping: " + str(opt.id_mapping))
    tdata_loader = CreateDataLoader(opt)
    tdataset = tdata_loader.load_data()
    tdataset_size = len(tdata_loader)

    # opt.model = "debug"
    # opt.name = "debug"

    # for i, data in enumerate(dataset):
    #     print(data["extrinsics"])


    for i, d in enumerate(zip(dataset, tdataset)):
        data, tdata = d
        if(i % 5 == 0):
            print(data["paths"])
            print(tdata["paths"])

            #print(data["TARGET"].shape)
            #print(data["UV"].shape)
            print(data["MASK"].shape)
            print(torch.unique(data["MASK"]))
            print(torch.unique(tdata["MASK"]))

           # print(data["paths"])
            f, ax = plt.subplots(5, 3, sharey=False)
            #imshow(data["TARGET"][0], ax1)
            #imshow(data["UV"][0], ax2)
            imshow(data["TARGET"][0], ax[0][0])
            imshow(tdata["TARGET"][0], ax[0][1])

            for i in range(1,5): 
            
                imshow(data["MASK"][0][i-1], ax[i][0], imgtype = "mask")
                imshow(tdata["MASK"][0][i-1], ax[i][1], imgtype = "mask")
                #imshow((data["MASK"][0][i-1] - tdata["MASK"][0][max(i-2,0)]), ax[i][2], imgtype = "mask")
                mapp = torch.abs(data["MASK"][0][i-1] - tdata["MASK"][0][max(i-2,0)]) -1
                imshow(mapp, ax[i][2], imgtype = "mask")

                # for t in range(0,5): 

                #     imshow(data["MASK"][0][i-1] == t, ax[i][2 + t], imgtype = "mask")
                #     imshow(tdata["MASK"][0][i-1] == t, ax[i][2 + 5 + t], imgtype = "mask")
            #print(data["UV"][0][0:2].shape)
            #imshow(data["UV"][0][0:2], imgtype = "uv")


            #imshow(data["TARGET"][0], imgtype="tensor")

            # masknp = data["MASK"][0].numpy()
            # print(np.unique(masknp))

            plt.show()
            print("-------------------------")

    exit()

    model = create_model(opt)
    model.setup(opt)

    # if opt.renderer != 'no_renderer':
    #     print('load renderer')
    #     model.loadModules(opt, opt.renderer, ['netD','netG'])

    visualizer = Visualizer(opt)
    total_steps = 0

    for epoch in range(opt.epoch_count, opt.niter + opt.niter_decay + 1):
        epoch_start_time = time.time()
        iter_data_time = time.time()
        epoch_iter = 0

        for i, data in enumerate(dataset):
            iter_start_time = time.time()
            if total_steps % opt.print_freq == 0:
                t_data = iter_start_time - iter_data_time
            visualizer.reset()
            total_steps += opt.batch_size
            epoch_iter += opt.batch_size
            
            model.set_input(data)
            model.optimize_parameters(epoch)

            if total_steps % opt.display_freq == 0:
                save_result = total_steps % opt.update_html_freq == 0
                visuals = model.get_current_visuals()


                visualizer.display_current_results(model.get_current_visuals(), epoch, save_result)

            if total_steps % opt.print_freq == 0:
                losses = model.get_current_losses()
                t = (time.time() - iter_start_time) / opt.batch_size
                visualizer.print_current_losses(epoch, epoch_iter, losses, t, t_data)
                if opt.display_id > 0:
                    visualizer.plot_current_losses(epoch, float(epoch_iter) / dataset_size, opt, losses)
                
            if total_steps % opt.save_latest_freq == 0:
                print('saving the latest model (epoch %d, total_steps %d)' % (epoch, total_steps))
                save_suffix = 'iter_%d' % total_steps if opt.save_by_iter else 'latest'
                model.save_networks(save_suffix)

            iter_data_time = time.time()
        if epoch % opt.save_epoch_freq == 0:
            print('saving the model at the end of epoch %d, iters %d' % (epoch, total_steps))
            model.save_networks('latest')
            model.save_networks(epoch)

        print('End of epoch %d / %d \t Time Taken: %d sec' %
              (epoch, opt.niter + opt.niter_decay, time.time() - epoch_start_time))
        model.update_learning_rate()


