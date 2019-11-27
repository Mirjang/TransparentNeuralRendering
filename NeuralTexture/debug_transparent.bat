@echo off

set DATASETS_DIR=C:\Users\Patrick\Desktop\NeuralTexture\TransparentNeuralRendering\Datasets
rem objects
rem set DATA=spheres_250v

set DATA=scene0_png


rem set DATASET_MODE=transparent
set DATASET_MODE=transparentPNG

rem number of objects in scene + 1 (background)
set NUM_OBJECTS=7



rem renderer
set RENDERER=MultiTarget-neuralRenderer_200

rem models
set MODEL=neuralRenderer
rem set MODEL=debug

rem rendering NN 
rem set RENDERER_TYPE=UNET_5_level
set RENDERER_TYPE=PerPixel_4

set TEX_DIM=256
set TEX_FEATURES=3
set NUM_DEPTH_LAYERS=8



set NAME=%MODEL%_%RENDERER_TYPE%_%DATA%_tex%TEX_DIM%x%TEX_FEATURES%x%NUM_OBJECTS%

rem #MODEL=pix2pix

rem optimizer parameters
set LR=0.0005
set BATCH_SIZE=1

rem GPU
set GPU_ID="0"

rem display params
set DISP_FREQ=100

rem --continue_train --epoch_count 8

python debug.py --continue_train --epoch_count 8 --niter 500 --save_epoch_freq 10 --rendererType %RENDERER_TYPE% --batch_size %BATCH_SIZE% --use_extrinsics --nObjects %NUM_OBJECTS% --tex_dim %TEX_DIM% --tex_features %TEX_FEATURES% --dataroot %DATASETS_DIR%/%DATA% --name %NAME% --num_depth_layers %NUM_DEPTH_LAYERS% --renderer %RENDERER% --model %MODEL% --netG unet_256 --lambda_L1 100 --dataset_mode %DATASET_MODE% --no_lsgan --norm batch --pool_size 0 --gpu_ids %GPU_ID% --lr %LR% --display_freq %DISP_FREQ% --print_freq %DISP_FREQ%
