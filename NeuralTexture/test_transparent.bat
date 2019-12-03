@echo off

set DATASETS_DIR=C:\Users\Patrick\Desktop\NeuralTexture\TransparentNeuralRendering\Datasets


rem renderer
set RENDERER=MultiTarget-neuralRenderer_200

rem optimizer parameters
set LR=0.001
set BATCH_SIZE=1

rem GPU
set GPU_ID="0"

rem display params
set DISP_FREQ=50
set LOSS=L1

set EPOCH=latest

call "./experiment_setups/LstmPerPixel4_lab_2.bat"
rem call "./experiment_setups/PerPixel4_lab_2.bat"
rem call "./experiment_setups/UNET_5_lab_2.bat"
rem call "./experiment_setups/Blend_lab_2.bat"
rem call "./experiment_setups/Debug.bat"


python test.py --nObjects %NUM_OBJECTS% --use_extrinsics --rendererType %RENDERER_TYPE% --num_depth_layers %NUM_DEPTH_LAYERS% --name %NAME% --epoch %EPOCH% --display_winsize 512 --nObjects %NUM_OBJECTS% --tex_dim %TEX_DIM% --tex_features %TEX_FEATURES% --dataroot %DATASETS_DIR%/%DATA%  --lossType %LOSS% --model %MODEL% --netG unet_256 --dataset_mode %DATASET_MODE% --norm batch  --gpu_ids %GPU_ID% %OPTIONS%