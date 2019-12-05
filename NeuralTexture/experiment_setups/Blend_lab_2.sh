set -ex

# dataset

DATA=lab_2

# choose between EXR and PNG loader
DATASET_MODE=transparent
# DATASET_MODE=transparentPNG


# number of objects in scene + 1 (background)
NUM_OBJECTS=7


# network used for rendering 
RENDERER_TYPE=Blend

# models -- for simple blending use debug
MODEL=neuralRenderer


# texture parameters
TEX_DIM=256
TEX_FEATURES=8
NUM_DEPTH_LAYERS=8

LR=0.1


# #experiment name
NAME=$(MODEL)_$(RENDERER_TYPE)_$(DATA)_tex$(TEX_DIM)x$(TEX_FEATURES)x$(NUM_OBJECTS)