import sys
import os
import numpy as np
import math
import cv2
import matplotlib.pyplot as plt

def psnr(img1, img2):
    mse = np.mean( (img1 - img2) ** 2 )
    if mse == 0:
        return 100
    PIXEL_MAX = 255.0
    return 20 * math.log10(PIXEL_MAX / math.sqrt(mse))

def main(): 

    if(len(sys.argv) < 2): 
        print("call this thing with file containing paths to frames")
        exit()

    with open(sys.argv[1]) as f: 
        dirs = f.readlines()
        print("comparing: " + str(dirs))

        nhf = len(dirs)+1
        if(len(sys.argv) > 2): 
            nhf = int(sys.argv[2])
        out_file = str(sys.argv[1]) + ".avi"
        if(os.path.exists(out_file)): 
            os.remove(out_file)

        loss_hist = None
        i = 0
        done = False
        while not done: 
            if(i%5==0): 
                print("Frame: " + str(i))
            gt_count = 0
            display = 0
            pdiri = ""
            gt = None
            frames = []
            for diri in dirs: 
                diri = diri.strip()

                if diri[:3] == "GT:": 
                    diri = diri[3:]
                    framediri = os.path.join(diri, str(i) + "_rgb_target.png")
                    gt_count += 1
                    if (not os.path.exists(framediri)):
                        print("Ended at: " + str(framediri))
                        done = True
                    else: 
                        gt = cv2.imread(framediri)
                elif diri == "GT": 
                    framediri = os.path.join(pdiri, str(i) + "_rgb_target.png")
                    gt_count += 1
                    if (not os.path.exists(framediri)):
                        print("Ended at: " + str(framediri))
                        done = True
                    else: 
                        gt = cv2.imread(framediri)

                else: 
                    framediri = os.path.join(diri, str(i) + "_rgb_fake.png")
                    pdiri = diri
                    if (not os.path.exists(framediri)):
                        print("Ended at: " + str(framediri))
                        done = True
                    else: 
                        frames.append(cv2.imread(framediri))
            
            if gt_count <1 : 
                gt = cv2.imread(os.path.join(diri, str(i) + "_rgb_target.png"))

            if not loss_hist: # 1st iter
                loss_hist = [[]*len(frames)]
            for i, frame in enumerate(frames): 
                err = psnr(frame, gt)
                loss_hist[i].append(err) 
            i+=1

    # done computing
    for i, losses in enumerate(loss_hist): 
        x = range(len(losses))    
        plt.plot(x, losses)
        plt.xlabel('Frame')
        plt.ylabel('PSNR')
        plt.savefig("psnr_" + str(i)+".png")
        plt.clf()
        avg = np.mean(losses)
        print("avg_psnr_" + str(i) + str(avg))
if __name__ == "__main__": 
    main()