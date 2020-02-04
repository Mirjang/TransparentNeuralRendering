set -ex

# dataset
export DATA=lab3

# choose between EXR and PNG loader
export DATASET_MODE=transparent

# models -- for simple blending use debug
export MODEL=neuralRenderer

# texture parameters
export TEX_DIM=256
export TEX_FEATURES=16
export NUM_DEPTH_LAYERS=16

# #experiment name
# additional options 
export OPTIONS="--ngf 256 --lossType all --lambda_L1 100 --lambda_VGG 100"
