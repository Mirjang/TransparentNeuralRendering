set -ex

# dataset
# DATA=spheres_250v
export DATA=chair

# choose between EXR and PNG loader
export DATASET_MODE=transparent
# DATASET_MODE=transparentPNG

# network used for rendering 
export RENDERER_TYPE=UNET_5_level

# models -- for simple blending use debug
export MODEL=neuralRenderer


# texture parameters
export TEX_DIM=512
export TEX_FEATURES=16
export NUM_DEPTH_LAYERS=6

export LR=0.001


# #experiment name
export NAME=${MODEL}_${RENDERER_TYPE}_${DATA}_tex${TEX_DIM}x${TEX_FEATURES}_L1VGG_DS2
# additional options 
export OPTIONS="--ngf 64 --use_extrinsics"

# continue training an existing model
export CONTINUE="--weight_decay 1e-5 --lossType all --lambda_L1 100 --lambda_VGG 100 --niter_decay 2000 --target_downsample_factor 1" # --continue_train --epoch_count 153 