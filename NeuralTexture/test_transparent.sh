set -ex

GPUID=0
DATASETS_DIR=./datasets

# dataset mode
DATASET_MODE=aligned

# objects
OBJECT=Macron

# renderer
RENDERER=$OBJECT

# neural texture
TEX_DIM=512
#TEX_DIM=1024
#TEX_DIM=2048
TEX_FEATURES=16

RENDERER_TYPE=UNET_5_level
#RENDERER_TYPE=UNET_8_level

# loss
#LOSS=VGG
LOSS=L1

# models
MODEL=neuralFaceRenderer
#MODEL=pix2pix

# optimizer parameters
LR=0.001

N_ITER=65
N_ITER_LR_DECAY=15

################################################################################
################################################################################
################################################################################

# train-single
NAME=Face-Hierarchy-$OBJECT-$MODEL-$TEX_DIM-$TEX_FEATURES-$RENDERER_TYPE-$LOSS
EROSION=1.0

# training
#python train.py --name $NAME --erosionFactor $EROSION --hierarchicalTex --tex_dim $TEX_DIM --tex_features $TEX_FEATURES --rendererType $RENDERER_TYPE --lossType $LOSS --display_env $OBJECT --niter $N_ITER --niter_decay $N_ITER_LR_DECAY --dataroot $DATASETS_DIR/$OBJECT --model $MODEL --netG unet_256 --lambda_L1 100 --dataset_mode $DATASET_MODE --no_lsgan --norm instance --pool_size 0  --gpu_ids $GPUID --lr $LR --batch_size 1
# testing
EPOCH=latest
python test.py --name $NAME --erosionFactor $EROSION --hierarchicalTex --epoch $EPOCH --display_winsize 512 --tex_dim $TEX_DIM --tex_features $TEX_FEATURES --rendererType $RENDERER_TYPE --lossType $LOSS --dataroot $DATASETS_DIR/$OBJECT --model $MODEL --netG unet_256 --dataset_mode $DATASET_MODE --norm instance  --gpu_ids $GPUID
#

################################################################################
################################################################################
################################################################################
