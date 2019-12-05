set -ex

CONTINUE=
OPTIONS=

DATASETS_DIR=/mnt/raid/patrickradner/datasets

# optimizer parameters
LR=0.001
BATCH_SIZE=1
# renderer
#RENDERER=MultiTarget-neuralRenderer_200
RENDERER=no_renderer
# GPU
GPU_ID="3"

# display params
DISP_FREQ=100

#source "./experiment_setups/LstmPerPixel4_lab_2.sh"
#source "./experiment_setups/PerPixel4_lab_2.sh"
#source "./experiment_setups/UNET_5_lab_2.sh"
source "./experiment_setups/Blend_lab_2.sh"
#source "./experiment_setups/Debug.sh"

python train.py --niter 10 --save_epoch_freq 20 --display_env $NAME --rendererType $RENDERER_TYPE --batch_size $BATCH_SIZE --nObjects $NUM_OBJECTS --tex_dim $TEX_DIM --tex_features $TEX_FEATURES --dataroot $DATASETS_DIR/$DATA --name $NAME --num_depth_layers $NUM_DEPTH_LAYERS --renderer $RENDERER --model $MODEL --netG unet_256 --lambda_L1 100 --dataset_mode $DATASET_MODE --no_lsgan --norm batch --pool_size 0 --gpu_ids $GPU_ID --lr $LR --display_freq $DISP_FREQ --print_freq $DISP_FREQ $CONTINUE $OPTIONS
