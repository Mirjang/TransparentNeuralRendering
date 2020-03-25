set -ex

# dataset
# DATA=spheres_250v
export DATA=tree

# choose between EXR and PNG loader
export DATASET_MODE=transparent
# DATASET_MODE=transparentPNG



# network used for rendering 
export RENDERER_TYPE=PerPixel2b_2

# models -- for simple blending use debug
export MODEL=neuralRenderer


# texture parameters
export TEX_DIM=512
export TEX_FEATURES=16
export NUM_DEPTH_LAYERS=8

export LR=0.00011


# #experiment name
export NAME=${MODEL}_${RENDERER_TYPE}_${DATA}_tex${TEX_DIM}x${TEX_FEATURES}_L1VGG
# additional options 
export OPTIONS="--use_extrinsics --ngf 512 --pad_front"

# continue training an existing model
export CONTINUE="--weight_decay 1e-3 --lossType GAN --lambda_L1 100 --lambda_VGG 100 --lambda_GAN 10 " # --continue_train --epoch_count 216"