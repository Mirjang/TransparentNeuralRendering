@echo off

set DATASETS_DIR=C:\Users\Patrick\Desktop\NeuralTexture\TransparentNeuralRendering\Datasets
rem objects
set DATA=spheres_250v
rem number of objects in scene + 1 (background)
set NUM_OBJECTS=4


rem renderer
set RENDERER=MultiTarget-neuralRenderer_200

rem models
set MODEL=neuralRenderer
set TEX_DIM=512
set TEX_FEATURES=8
set NUM_DEPTH_LAYERS=6

rem #MODEL=pix2pix

rem optimizer parameters
set LR=0.005
set BATCH_SIZE=1

rem GPU
set GPU_ID=0


rem display params
set DISP_FREQ=25


python debug.py --batch_size %BATCH_SIZE% --niter 500 --nObjects %NUM_OBJECTS% --tex_dim %TEX_DIM% --tex_features %TEX_FEATURES% --dataroot %DATASETS_DIR%/%DATA% --name %MODEL% --num_depth_layers %NUM_DEPTH_LAYERS% --renderer %RENDERER% --model %MODEL% --netG unet_256 --lambda_L1 100 --dataset_mode transparent --no_lsgan --norm batch --pool_size 0 --gpu_ids %GPU_ID% --lr %LR% --display_freq %DISP_FREQ% --update_html_freq %DISP_FREQ% --print_freq %DISP_FREQ%
