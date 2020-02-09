set -ex

# dataset
# DATA=spheres_250v
export DATA=eztree

# choose between EXR and PNG loader
export DATASET_MODE=transparent
# DATASET_MODE=transparentPNG

# network used for rendering 
export RENDERER_TYPE=PerPixel2_2

# models -- for simple blending use debug
export MODEL=neuralRenderer


# texture parameters
export TEX_DIM=1024
export TEX_FEATURES=32
export NUM_DEPTH_LAYERS=4

export LR=0.001


# #experiment name
export NAME=${MODEL}_${RENDERER_TYPE}_${DATA}_tex${TEX_DIM}x${TEX_FEATURES}_L1VGG
# additional options 
export OPTIONS="--use_extrinsics --ngf 128 "

# continue training an existing model
export CONTINUE="--weight_decay 1e-5 --lossType all --lambda_L1 100 --lambda_VGG 100 --niter_decay 2000" # --continue_train --epoch_count 193 