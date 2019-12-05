set -ex

DATASETS_DIR=/mnt/raid/patrickradner/datasets


 #renderer
#RENDERER=MultiTarget-neuralRenderer_200
RENDERER=no_renderer

 #optimizer parameters
LR=0.001
shCH_SIZE=1

 #GPU
GPU_ID="0"

 #display params
DISP_FREQ=50
LOSS=L1

EPOCH=latest

source "./experiment_setups/LstmPerPixel4_lab_2.sh"
 #source "./experiment_setups/PerPixel4_lab_2.sh"
 #source "./experiment_setups/UNET_5_lab_2.sh"
 #source "./experiment_setups/Blend_lab_2.sh"
 #source "./experiment_setups/Debug.sh"


python test.py --nObjects $NUM_OBJECTS --use_extrinsics --rendererType $RENDERER_TYPE --num_depth_layers $NUM_DEPTH_LAYERS --name $NAME --epoch $EPOCH --display_winsize 512 --nObjects $NUM_OBJECTS --tex_dim $TEX_DIM --tex_features $TEX_FEATURES --dataroot $DATASETS_DIR/$DATA  --lossType $LOSS --model $MODEL --netG unet_256 --dataset_mode $DATASET_MODE --norm batch  --gpu_ids $GPU_ID $OPTIONS
