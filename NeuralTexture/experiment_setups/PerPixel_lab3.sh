set -ex

# dataset
# DATA=spheres_250v
export DATA=lab3

# choose between EXR and PNG loader
export DATASET_MODE=transparent
# DATASET_MODE=transparentPNG



# network used for rendering 
export RENDERER_TYPE=PerPixel_8

# models -- for simple blending use debug
export MODEL=neuralRenderer


# texture parameters
export TEX_DIM=256
export TEX_FEATURES=16
export NUM_DEPTH_LAYERS=16

export LR=0.001


# #experiment name
export NAME=${MODEL}_${RENDERER_TYPE}_${DATA}_tex${TEX_DIM}x${TEX_FEATURES}_v2
# additional options 
export OPTIONS="--use_extrinsics --ngf 256 "

# continue training an existing model
CONTINUE="--weight_decay 10e-2"
# CONTINUE="--continue_train --epoch_count 11"