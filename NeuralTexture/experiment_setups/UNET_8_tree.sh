set -ex

# dataset
# DATA=spheres_250v
export DATA=tree

# choose between EXR and PNG loader
export DATASET_MODE=transparent
# DATASET_MODE=transparentPNG

# network used for rendering 
export RENDERER_TYPE=UNET_8_level

# models -- for simple blending use debug
export MODEL=neuralRenderer


# texture parameters
export TEX_DIM=512
export TEX_FEATURES=16
export NUM_DEPTH_LAYERS=8

export LR=0.001


# #experiment name
export NAME=${MODEL}_${RENDERER_TYPE}_${DATA}_tex${TEX_DIM}x${TEX_FEATURES}_L1VGG
# additional options 
export OPTIONS="--use_extrinsics" # --ngf 256 "

# continue training an existing model
export CONTINUE="--weight_decay 1e-3 --lossType all --lambda_L1 100 --lambda_VGG 100" # --continue_train --epoch_count 216"