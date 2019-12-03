@echo off

rem set dataset
rem set DATA=spheres_250v
set DATA=lab_2

rem choose between EXR and PNG loader
set DATASET_MODE=transparent
rem set DATASET_MODE=transparentPNG


rem number of objects in scene + 1 (background)
set NUM_OBJECTS=7


rem network used for rendering 
set RENDERER_TYPE=LstmPerPixel_2

rem models -- for simple blending use debug
set MODEL=neuralRenderer


rem texture parameters
set TEX_DIM=256
set TEX_FEATURES=8
set NUM_DEPTH_LAYERS=8

set LR=0.001


rem #experiment name
set NAME=%MODEL%_%RENDERER_TYPE%_%DATA%_tex%TEX_DIM%x%TEX_FEATURES%x%NUM_OBJECTS%

rem additional options 
rem set OPTIONS=--use_extrinsics
set OPTIONS=--use_extrinsics --ngf 32

rem continue training an existing model
set CONTINUE=--continue_train --epoch_count 31