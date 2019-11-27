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

rem set RENDERER_TYPE=UNET_5_level
set RENDERER_TYPE=PerPixel_4

rem models
set MODEL=neuralRenderer
rem set MODEL=debug

set TEX_DIM=256
set TEX_FEATURES=3
set NUM_DEPTH_LAYERS=8



rem #MODEL=pix2pix
set NAME=%MODEL%_%RENDERER_TYPE%_%DATA%_tex%TEX_DIM%x%TEX_FEATURES%x%NUM_OBJECTS%

rem optimizer parameters
set LR=0.001
set BATCH_SIZE=1

rem GPU
set GPU_ID=0

rem display params
set DISP_FREQ=50
set LOSS=L1

set EPOCH=latest
python test.py --nObjects %NUM_OBJECTS% --use_extrinsics --rendererType %RENDERER_TYPE% --num_depth_layers %NUM_DEPTH_LAYERS% --name %NAME% --epoch %EPOCH% --display_winsize 512 --nObjects %NUM_OBJECTS% --tex_dim %TEX_DIM% --tex_features %TEX_FEATURES% --dataroot %DATASETS_DIR%/%DATA%  --lossType %LOSS% --model %MODEL% --netG unet_256 --dataset_mode %DATASET_MODE% --norm batch  --gpu_ids %GPU_ID%