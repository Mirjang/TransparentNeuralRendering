set -ex

CONTINUE=
OPTIONS=

DATASETS_DIR=/mnt/raid/patrickradner/datasets
RESULTS_DIR=/mnt/raid/patrickradner/cmp_data/results
CHECKPOINTS_DIR=/mnt/raid/patrickradner/cmp_data/checkpoints
#source "./experiment_setups/model_comparison/L1_tex256x8_lab3.sh"
source "./experiment_setups/model_comparison/VGG_tex256x8_lab3.sh"

# optimizer parameters
LR=0.001
BATCH_SIZE=1
# renderer
#RENDERER=MultiTarget-neuralRenderer_200
RENDERER=no_renderer
# GPU
GPU_ID="0"

# display params
DISP_FREQ=100
LOSS=L1
EPOCH=latest

# network used for rendering 
#RENDERER_TYPE=UNET_3_level #gpu1
#RENDERER_TYPE=UNET_5_level #gpu1
RENDERER_TYPE=PerPixel2_3 #gpu1

#RENDERER_TYPE=PerPixel_4 #not int use atm

#CONTINUE="--continue_train --epoch_count 30"



if [[ $(nvidia-smi | grep "^|    $GPU_ID    ") ]]; then
    read -p "GPU currently in use, continue? " -n 1 -r
    echo
    if [[ $REPLY =~ ^[Nn]$ ]]
    then
        exit 1
    fi
fi



PHASE=train_random_distance
NAME=${RENDERER_TYPE}_${DATA}RND_tex${TEX_DIM}x${TEX_FEATURES}_L1VGG
#python train.py --niter 80 --save_epoch_freq 20 --phase $PHASE --display_env $NAME --rendererType $RENDERER_TYPE --batch_size $BATCH_SIZE --tex_dim $TEX_DIM --tex_features $TEX_FEATURES --dataroot $DATASETS_DIR/$DATA --name $NAME --num_depth_layers $NUM_DEPTH_LAYERS --renderer $RENDERER --model $MODEL --netG unet_256 --dataset_mode $DATASET_MODE --no_lsgan --norm batch --pool_size 0 --gpu_ids $GPU_ID --lr $LR --display_freq $DISP_FREQ --checkpoints_dir $CHECKPOINTS_DIR --print_freq $DISP_FREQ $CONTINUE $OPTIONS
#python test.py --rendererType $RENDERER_TYPE --num_depth_layers $NUM_DEPTH_LAYERS --name $NAME --epoch $EPOCH --results_dir $RESULTS_DIR --checkpoints_dir $CHECKPOINTS_DIR --display_winsize 512 --tex_dim $TEX_DIM --tex_features $TEX_FEATURES --dataroot $DATASETS_DIR/$DATA  --lossType $LOSS --model $MODEL --netG unet_256 --dataset_mode $DATASET_MODE --norm batch --gpu_ids $GPU_ID $OPTIONS
#python test.py --rendererType $RENDERER_TYPE --phase test_interpolating --num_depth_layers $NUM_DEPTH_LAYERS --name $NAME --epoch $EPOCH --results_dir $RESULTS_DIR --checkpoints_dir $CHECKPOINTS_DIR --display_winsize 512 --tex_dim $TEX_DIM --tex_features $TEX_FEATURES --dataroot $DATASETS_DIR/$DATA  --lossType $LOSS --model $MODEL --netG unet_256 --dataset_mode $DATASET_MODE --norm batch --gpu_ids $GPU_ID $OPTIONS

#--id_mapping "0,1,8,2,3,4,5,6,7"
PHASE=train_random_positions
NAME=${RENDERER_TYPE}_${DATA}RNDPOS_tex${TEX_DIM}x${TEX_FEATURES}_L1VGG
#python train.py --niter 80 --save_epoch_freq 20 --phase $PHASE --display_env $NAME --rendererType $RENDERER_TYPE --batch_size $BATCH_SIZE --tex_dim $TEX_DIM --tex_features $TEX_FEATURES --dataroot $DATASETS_DIR/$DATA --name $NAME --num_depth_layers $NUM_DEPTH_LAYERS --renderer $RENDERER --model $MODEL --netG unet_256 --dataset_mode $DATASET_MODE --no_lsgan --norm batch --pool_size 0 --gpu_ids $GPU_ID --lr $LR --display_freq $DISP_FREQ --checkpoints_dir $CHECKPOINTS_DIR --print_freq $DISP_FREQ $CONTINUE $OPTIONS
#python test.py --rendererType $RENDERER_TYPE --phase test_local --num_depth_layers $NUM_DEPTH_LAYERS --name $NAME --epoch $EPOCH --results_dir $RESULTS_DIR --checkpoints_dir $CHECKPOINTS_DIR --display_winsize 512 --tex_dim $TEX_DIM --tex_features $TEX_FEATURES --dataroot $DATASETS_DIR/$DATA  --lossType $LOSS --model $MODEL --netG unet_256 --dataset_mode $DATASET_MODE --norm batch --gpu_ids $GPU_ID $OPTIONS
python test.py --rendererType $RENDERER_TYPE --phase test_interpolating_local --num_depth_layers $NUM_DEPTH_LAYERS --name $NAME --epoch $EPOCH --results_dir $RESULTS_DIR --checkpoints_dir $CHECKPOINTS_DIR --display_winsize 512 --tex_dim $TEX_DIM --tex_features $TEX_FEATURES --dataroot $DATASETS_DIR/$DATA  --lossType $LOSS --model $MODEL --netG unet_256 --dataset_mode $DATASET_MODE --norm batch --gpu_ids $GPU_ID $OPTIONS
 