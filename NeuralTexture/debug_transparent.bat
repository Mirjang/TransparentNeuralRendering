@echo off

set DATASETS_DIR=C:\Users\Patrick\Desktop\NeuralTexture\TransparentNeuralRendering\Data
rem objects
set DATA=sphere_mv_bg
set NUM_OBJECTS=4


rem renderer
set RENDERER=MultiTarget-neuralRenderer_200

rem models
set MODEL=neuralRenderer
set TEX_DIM=512
set TEX_FEATURES=8

rem #MODEL=pix2pix

rem optimizer parameters
set LR=0.005
set BATCH_SIZE=4

rem GPU
set GPU_ID=0

python debug.py --batch_size %BATCH_SIZE% --niter 500 --nObjects %NUM_OBJECTS% --tex_dim %TEX_DIM% --tex_features %TEX_FEATURES% --dataroot %DATASETS_DIR%/%DATA% --name %MODEL% --renderer %RENDERER% --model %MODEL% --netG unet_256 --lambda_L1 100 --dataset_mode transparent --no_lsgan --norm batch --pool_size 0 --gpu_ids %GPU_ID% --lr %LR%
