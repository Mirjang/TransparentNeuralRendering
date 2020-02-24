set -ex

# dataset
# DATA=spheres_250v
export DATA=eztree

# choose between EXR and PNG loader
export DATASET_MODE=transparent
# DATASET_MODE=transparentPNG

# network used for rendering 
export RENDERER_TYPE=PerPixel2b_2

# models -- for simple blending use debug
export MODEL=neuralRenderer


# texture parameters
export TEX_DIM=512
export TEX_FEATURES=64
export NUM_DEPTH_LAYERS=4

export LR=0.0004


# #experiment name
export NAME=${MODEL}_${RENDERER_TYPE}_${DATA}_tex${TEX_DIM}x${TEX_FEATURES}_GAN
# additional options 
export OPTIONS="--use_extrinsics --ngf 512 --pad_front"

# continue training an existing model
export CONTINUE="--weight_decay 1e-4 --lossType GAN --lambda_L1 100 --lambda_GAN 10 --suspend_gan_epochs 25 --niter_decay 4000" # --continue_train --epoch_count 216"