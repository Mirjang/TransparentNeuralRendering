set -ex

# dataset
# DATA=spheres_250v
export DATA=chair

# choose between EXR and PNG loader
export DATASET_MODE=transparent
# DATASET_MODE=transparentPNG

# network used for rendering 
export RENDERER_TYPE=PerPixel2b_3

# models -- for simple blending use debug
export MODEL=neuralRenderer


# texture parameters
export TEX_DIM=512
export TEX_FEATURES=16
export NUM_DEPTH_LAYERS=6

export LR=0.0004


# #experiment name
export NAME=${MODEL}_${RENDERER_TYPE}_${DATA}_tex${TEX_DIM}x${TEX_FEATURES}_GAN_PAD
# additional options 
export OPTIONS="--ngf 256 --use_extrinsics --pad_front"

# continue training an existing model
export CONTINUE="--weight_decay 1e-4 --lossType GAN --suspend_gan_epochs 25 --lambda_L1 100 --lambda_GAN 10 --niter_decay 2000" # --continue_train --epoch_count 10" --target_downsample_factor 2