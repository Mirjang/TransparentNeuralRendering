@echo off

set DATASETS_DIR=C:\Users\Patrick\Desktop\NeuralTexture\TransparentNeuralRendering\Data
rem objects
set DATA=debug

rem renderer
set RENDERER=MultiTarget-neuralRenderer_200

rem models
set MODEL=neuralRenderer

rem #MODEL=pix2pix

rem optimizer parameters
set LR=0.05

rem GPU
set GPU_ID=0

python debug.py --fix_renderer --niter 2000 --dataroot %DATASETS_DIR%/%DATA% --name %MODEL% --renderer %RENDERER% --model %MODEL% --netG unet_256 --lambda_L1 100 --dataset_mode transparent --no_lsgan --norm batch --pool_size 0 --gpu_ids %GPU_ID% --lr %LR%
