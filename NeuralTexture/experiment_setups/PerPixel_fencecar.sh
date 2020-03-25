set -ex

# dataset
# DATA=spheres_250v
export DATA=fencecar_nobb

# choose between EXR and PNG loader
export DATASET_MODE=transparent
# DATASET_MODE=transparentPNG

# network used for rendering 
export RENDERER_TYPE=PerPixel2b_2

# models -- for simple blending use debug
export MODEL=neuralRenderer


# texture parameters
export TEX_DIM=512
export TEX_FEATURES=32
export NUM_DEPTH_LAYERS=6

export LR=0.001


# #experiment name
export NAME=${MODEL}_${RENDERER_TYPE}_${DATA}_tex${TEX_DIM}x${TEX_FEATURES}_L1VGG
# additional options 
export OPTIONS="--ngf 128 --target_downsample_factor 1 --pad_front"

# continue training an existing model
export CONTINUE="--weight_decay 1e-5 --lossType all --lambda_L1 100 --lambda_VGG 100 --niter_decay 2000" # --continue_train --epoch_count 153 