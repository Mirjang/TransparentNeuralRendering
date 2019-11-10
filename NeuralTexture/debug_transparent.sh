set -ex

DATASETS_DIR=/home/mirjang/Desktop/NeuralTextures/TransparentNeuralRendering/Data
# objects
DATA=debug
NUM_OBJECTS=3 
# renderer
RENDERER=MultiTarget-neuralRenderer_200

# models
MODEL=neuralRenderer
#MODEL=pix2pix

# optimizer parameters
LR=0.001

# GPU
GPU_ID=0

python debug.py --nObjects $NUM_OBJECTS --fix_renderer --niter 2000 --dataroot $DATASETS_DIR/$DATA --name $OBJECT-$MODEL --renderer $RENDERER --model $MODEL --netG unet_256 --lambda_L1 100 --dataset_mode transparent --no_lsgan --norm batch --pool_size 0 --gpu_ids $GPU_ID --lr $LR
