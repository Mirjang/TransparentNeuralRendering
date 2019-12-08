set -ex

# dataset
# DATA=spheres_250v
export DATA=lab2

# choose between EXR and PNG loader
export DATASET_MODE=transparent
# DATASET_MODE=transparentPNG


# number of objects in scene + 1 (background)
export NUM_OBJECTS=7


# network used for rendering 
export RENDERER_TYPE=LstmUNET_2_3

# models -- for simple blending use debug
export MODEL=neuralRenderer


# texture parameters
export TEX_DIM=256
export TEX_FEATURES=8
export NUM_DEPTH_LAYERS=8

export LR=0.001


# #experiment name
NAME=${MODEL}_${RENDERER_TYPE}_${DATA}_tex${TEX_DIM}x${TEX_FEATURES}x${NUM_OBJECTS}
# additional options 
# OPTIONS=--use_extrinsics
export OPTIONS="--use_extrinsics --ngf 32"

# continue training an existing model
CONTINUE="--continue_train --epoch_count 5"
