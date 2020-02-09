set -ex

# dataset
export DATA=lab3

# choose between EXR and PNG loader
export DATASET_MODE=transparent
# DATASET_MODE=transparentPNG
# network used for rendering 
export RENDERER_TYPE=GruPerPixel_1_1

# models -- for simple blending use debug
export MODEL=neuralRenderer


# texture parameters
export TEX_DIM=256
export TEX_FEATURES=8
export NUM_DEPTH_LAYERS=16 # reduce this as much as possible bc. rnns eat um mem like crazy...

export LR=0.001


# #experiment name
NAME=${MODEL}_${RENDERER_TYPE}_${DATA}_tex${TEX_DIM}x${TEX_FEATURES}_L1VGG
# additional options 
export OPTIONS="--use_extrinsics --extrinsics_skip 2 --nref 16 --nrhf 16 --nrdf=64"
export CONTINUE="--lossType all --lambda_L1 100 --lambda_VGG 100 --niter_decay 1000"
# continue training an existing model
#CONTINUE="--continue_train --epoch_count 47"
