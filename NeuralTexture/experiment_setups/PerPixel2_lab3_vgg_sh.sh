set -ex

# dataset
# DATA=spheres_250v
export DATA=lab3

# choose between EXR and PNG loader
export DATASET_MODE=transparent
# DATASET_MODE=transparentPNG



# network used for rendering 
export RENDERER_TYPE=PerPixel2b_3

# models -- for simple blending use debug
export MODEL=neuralRenderer


# texture parameters
export TEX_DIM=256
export TEX_FEATURES=16
export NUM_DEPTH_LAYERS=16

export LR=0.0004


# #experiment name
export NAME=${MODEL}_${RENDERER_TYPE}_${DATA}_tex${TEX_DIM}x${TEX_FEATURES}_L1VGG_SH
# additional options 
export OPTIONS="--use_spherical_harmonics --ngf 256"

# continue training an existing model
export CONTINUE="--lossType all --suspend_gan_epochs 25 --lambda_L1 100 --lambda_VGG 100 --niter_decay 2000" # --continue_train --epoch_count 216"