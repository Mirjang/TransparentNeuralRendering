import sys
import os

import cv2

def main(): 

    if(len(sys.argv) < 2): 
        print("call this thing with file containing paths to frames")
        exit()

    with open(sys.argv[1]) as f: 
        dirs = f.readlines()
        print("comparing: " + str(dirs))

        nhf = len(dirs)
        if(len(sys.argv) > 2): 
            nhf = int(sys.argv[2])
        out_file = str(sys.argv[1]) + ".avi"
        if(os.path.exists(out_file)): 
            os.remove(out_file)

        i = 0
        writer = None
        while True: 
            frames = []
            layers = []
            if(i%5==0): 
                print("Frame: " + str(i))
            x = 0
            display = 0
            for diri in dirs: 
                diri = diri.strip()

                framediri = os.path.join(diri, str(i) + "_rgb_fake.png")
                if (not os.path.exists(framediri)):
                    print("Ended at: " + str(framediri))
                    writer.release()
                    exit()
                im = cv2.imread(framediri)
                if(x==0): 
                    display = im
                elif x == nhf-1: 
                    display = cv2.hconcat([display, im])
                    layers.append(display)
                    x = -1
                else: 
                    display = cv2.hconcat([display, im])
                x+=1
            im = cv2.imread(os.path.join(diri, str(i) + "_rgb_target.png"))
            display = cv2.hconcat([display, im])
            layers.append(display)
            display = cv2.vconcat(layers)

            if(writer == None): 
                dims = display.shape[1], display.shape[0]
                print("Dims: " + str(dims))
                print(len(layers))
                writer = cv2.VideoWriter(out_file, cv2.VideoWriter_fourcc(*"XVID"), 15.0, dims, True)

            writer.write(display)
            i+=1

if __name__ == "__main__": 
    main()