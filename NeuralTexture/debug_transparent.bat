@echo off


call "C:\Users\Patrick\Anaconda3\Scripts\activate.bat"

python debug.py --rendererType neuralRenderer --num_depth_layers 6 --name d --epoch 9 --display_winsize 512 --tex_dim 512 --tex_features 16 --dataroot D:/datasets/fencecar  --lossType L1 --model neuralRenderer --netG unet_256 --dataset_mode transparent --norm batch --gpu_ids 0
