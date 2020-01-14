set -ex

# dataset
# DATA=spheres_250v
export DATA=lab3

# choose between EXR and PNG loader
export DATASET_MODE=transparent

# network used for rendering 
export RENDERER_TYPE=UNET_5_level

# models -- for simple blending use debug
export MODEL=neuralRenderer

# texture parameters
export TEX_DIM=256
export TEX_FEATURES=8
export NUM_DEPTH_LAYERS=1

# #experiment name
export NAME=UNET_BASELINE_1L
# additional options 
export OPTIONS=--use_extrinsics

# continue training an existing model
#CONTINUE="--continue_train --epoch_count 81"
