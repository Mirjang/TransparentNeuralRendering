set -ex

# dataset
# DATA=spheres_250v
export DATA=tree

# choose between EXR and PNG loader
export DATASET_MODE=transparent
# DATASET_MODE=transparentPNG

# network used for rendering 
export RENDERER_TYPE=PerPixel2_4

# models -- for simple blending use debug
export MODEL=neuralRenderer


# texture parameters
export TEX_DIM=1024
export TEX_FEATURES=16
export NUM_DEPTH_LAYERS=8

export LR=0.0001


# #experiment name
export NAME=${MODEL}_${RENDERER_TYPE}_${DATA}_tex${TEX_DIM}x${TEX_FEATURES}_L1VGG
# additional options 
export OPTIONS="--ngf 256 --use_extrinsics"

# continue training an existing model
export CONTINUE="--weight_decay 1e-3 --lossType all --lambda_L1 100 --lambda_VGG 100" # --continue_train --epoch_count 216"