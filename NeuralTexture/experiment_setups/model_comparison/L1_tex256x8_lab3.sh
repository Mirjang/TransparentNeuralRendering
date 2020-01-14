set -ex

# dataset
# DATA=spheres_250v
export DATA=lab3

# choose between EXR and PNG loader
export DATASET_MODE=transparent


# models -- for simple blending use debug
export MODEL=neuralRenderer

# texture parameters
export TEX_DIM=256
export TEX_FEATURES=8
export NUM_DEPTH_LAYERS=16

# #experiment name
# additional options 
export OPTIONS=--use_extrinsics

# continue training an existing model
#CONTINUE="--continue_train --epoch_count 81"
