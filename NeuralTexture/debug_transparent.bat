@echo off

set DATASETS_DIR=C:\Users\Patrick\Desktop\NeuralTexture\TransparentNeuralRendering\Datasets
rem objects
rem set DATA=spheres_250v
set DATA=nightstand_glasses_trashcan

rem number of objects in scene + 1 (background)
set NUM_OBJECTS=7


rem renderer
set RENDERER=MultiTarget-neuralRenderer_200

rem models
set MODEL=neuralRenderer
rem set MODEL=debug

set TEX_DIM=256
set TEX_FEATURES=16
set NUM_DEPTH_LAYERS=8



rem #MODEL=pix2pix

rem optimizer parameters
set LR=0.001
set BATCH_SIZE=1

rem GPU
set GPU_ID=0

rem display params
set DISP_FREQ=100


python debug.py --niter 500 --save_epoch_freq 10 --batch_size %BATCH_SIZE%  --nObjects %NUM_OBJECTS% --tex_dim %TEX_DIM% --tex_features %TEX_FEATURES% --dataroot %DATASETS_DIR%/%DATA% --name %MODEL% --num_depth_layers %NUM_DEPTH_LAYERS% --renderer %RENDERER% --model %MODEL% --netG unet_256 --lambda_L1 100 --dataset_mode transparent --no_lsgan --norm batch --pool_size 0 --gpu_ids %GPU_ID% --lr %LR% --display_freq %DISP_FREQ% --update_html_freq %DISP_FREQ% --print_freq %DISP_FREQ%
