@echo off

set CONTINUE=
set OPTIONS=

set DATASETS_DIR=C:\Users\Patrick\Desktop\NeuralTexture\TransparentNeuralRendering\Datasets

rem optimizer parameters
set LR=0.001
set BATCH_SIZE=1
rem renderer
set RENDERER=MultiTarget-neuralRenderer_200
rem GPU
set GPU_ID="0"

rem display params
set DISP_FREQ=100


call "./experiment_setups/PerPixel4_lab_2.bat"
rem call "./experiment_setups/UNET_5_lab_2.bat"
rem call "./experiment_setups/Blend_lab_2.bat"
rem call "./experiment_setups/Debug.bat"


python debug.py --niter 500 --save_epoch_freq 10 --rendererType %RENDERER_TYPE% --batch_size %BATCH_SIZE% --nObjects %NUM_OBJECTS% --tex_dim %TEX_DIM% --tex_features %TEX_FEATURES% --dataroot %DATASETS_DIR%/%DATA% --name %NAME% --num_depth_layers %NUM_DEPTH_LAYERS% --renderer %RENDERER% --model %MODEL% --netG unet_256 --lambda_L1 100 --dataset_mode %DATASET_MODE% --no_lsgan --norm batch --pool_size 0 --gpu_ids %GPU_ID% --lr %LR% --display_freq %DISP_FREQ% --print_freq %DISP_FREQ% %CONTINUE% %OPTIONS%
