import numpy as np
import os
import sys
import ntpath
import time
from . import util
from . import html
from scipy.misc import imresize
#from PIL import Image


from cv2 import VideoWriter, VideoWriter_fourcc, imread, resize



class VideoOutput(): 

    def __init__(self, opt): 
        self.out_dir = os.path.join(opt.results_dir, opt.name, '%s_%s' % (opt.phase, opt.epoch))
        self.out_file = os.path.join(self.out_dir, "real_fake.avi")
        self.writer = None


    def writeFrame(self, visuals): 

        ims, txts, links = [], [], []

    i = 0

    for label, im_data in visuals.items():
        im = util.tensor2im(im_data)

        if(i==0): 
            display = im
        else: 
            display = np.concatenate((display, im))
        i+=1
        

    if(self.writer == None): 
        dims = display.shape[1], display.shape[0]
        self.writer = VideoWriter(self.out_file, VideoWriter_fourcc("XVID"), 30, dims, True)

    vid.write(display)





    def close(self): 
        self.writer.release()