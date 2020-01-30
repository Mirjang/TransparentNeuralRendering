set -ex

CONTINUE=
OPTIONS=

DATASETS_DIR=/mnt/raid/patrickradner/datasets
RESULTS_DIR=/mnt/raid/patrickradner/cmp_extrinsics/results
CHECKPOINTS_DIR=/mnt/raid/patrickradner/cmp_extrinsics/checkpoints
source "./experiment_setups/model_comparison/extrinsics_setup_VGG.sh"

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
LOSS=L1
EPOCH=latest

# network used for rendering 
RENDERER_TYPE=PerPixel2_4 #gpu1


#EXTRINSICS="--no_extrinsics"
#EXTRINSICS="--use_extrinsics"
#EXTRINSICS="--use_spherical_harmonics"
EXTRINSICS="--YOUFORGOTTOSETEXTRINSICS!!!!!"

#RENDERER_TYPE=PerPixel_4 #not int use atm

#CONTINUE="--continue_train --epoch_count 30"

NAME=${RENDERER_TYPE}_${DATA}_tex${TEX_DIM}x${TEX_FEATURES}${EXTRINSICS}


if [[ $(nvidia-smi | grep "^|    $GPU_ID    ") ]]; then
    read -p "GPU currently in use, continue? " -n 1 -r
    echo
    if [[ $REPLY =~ ^[Nn]$ ]]
    then
        exit 1
    fi
fi


EXTRINSICS="--no_extrinsics"
NAME=${RENDERER_TYPE}_${DATA}_tex${TEX_DIM}x${TEX_FEATURES}${EXTRINSICS}
# python train.py --niter 100 $EXTRINSICS --save_epoch_freq 20 --display_env $NAME --rendererType $RENDERER_TYPE --batch_size $BATCH_SIZE --tex_dim $TEX_DIM --tex_features $TEX_FEATURES --dataroot $DATASETS_DIR/$DATA --name $NAME --num_depth_layers $NUM_DEPTH_LAYERS --renderer $RENDERER --model $MODEL --netG unet_256 --dataset_mode $DATASET_MODE --no_lsgan --norm batch --pool_size 0 --gpu_ids $GPU_ID --lr $LR --display_freq $DISP_FREQ --checkpoints_dir $CHECKPOINTS_DIR --print_freq $DISP_FREQ $CONTINUE $OPTIONS
python test.py --rendererType $RENDERER_TYPE $EXTRINSICS --num_depth_layers $NUM_DEPTH_LAYERS --name $NAME --epoch $EPOCH --results_dir $RESULTS_DIR --checkpoints_dir $CHECKPOINTS_DIR --display_winsize 512 --tex_dim $TEX_DIM --tex_features $TEX_FEATURES --dataroot $DATASETS_DIR/$DATA  --lossType $LOSS --model $MODEL --netG unet_256 --dataset_mode $DATASET_MODE --norm batch --gpu_ids $GPU_ID $OPTIONS
# python test.py --rendererType $RENDERER_TYPE $EXTRINSICS --phase test_interpolating --num_depth_layers $NUM_DEPTH_LAYERS --name $NAME --epoch $EPOCH --results_dir $RESULTS_DIR --checkpoints_dir $CHECKPOINTS_DIR --display_winsize 512 --tex_dim $TEX_DIM --tex_features $TEX_FEATURES --dataroot $DATASETS_DIR/$DATA  --lossType $LOSS --model $MODEL --netG unet_256 --dataset_mode $DATASET_MODE --norm batch --gpu_ids $GPU_ID $OPTIONS


EXTRINSICS="--use_extrinsics"
NAME=${RENDERER_TYPE}_${DATA}_tex${TEX_DIM}x${TEX_FEATURES}${EXTRINSICS}
# python train.py --niter 100 $EXTRINSICS --save_epoch_freq 20 --display_env $NAME --rendererType $RENDERER_TYPE --batch_size $BATCH_SIZE --tex_dim $TEX_DIM --tex_features $TEX_FEATURES --dataroot $DATASETS_DIR/$DATA --name $NAME --num_depth_layers $NUM_DEPTH_LAYERS --renderer $RENDERER --model $MODEL --netG unet_256 --dataset_mode $DATASET_MODE --no_lsgan --norm batch --pool_size 0 --gpu_ids $GPU_ID --lr $LR --display_freq $DISP_FREQ --checkpoints_dir $CHECKPOINTS_DIR --print_freq $DISP_FREQ $CONTINUE $OPTIONS
#python test.py --rendererType $RENDERER_TYPE $EXTRINSICS --num_depth_layers $NUM_DEPTH_LAYERS --name $NAME --epoch $EPOCH --results_dir $RESULTS_DIR --checkpoints_dir $CHECKPOINTS_DIR --display_winsize 512 --tex_dim $TEX_DIM --tex_features $TEX_FEATURES --dataroot $DATASETS_DIR/$DATA  --lossType $LOSS --model $MODEL --netG unet_256 --dataset_mode $DATASET_MODE --norm batch --gpu_ids $GPU_ID $OPTIONS
# python test.py --rendererType $RENDERER_TYPE $EXTRINSICS --phase test_interpolating --num_depth_layers $NUM_DEPTH_LAYERS --name $NAME --epoch $EPOCH --results_dir $RESULTS_DIR --checkpoints_dir $CHECKPOINTS_DIR --display_winsize 512 --tex_dim $TEX_DIM --tex_features $TEX_FEATURES --dataroot $DATASETS_DIR/$DATA  --lossType $LOSS --model $MODEL --netG unet_256 --dataset_mode $DATASET_MODE --norm batch --gpu_ids $GPU_ID $OPTIONS


EXTRINSICS="--use_spherical_harmonics"
NAME=${RENDERER_TYPE}_${DATA}_tex${TEX_DIM}x${TEX_FEATURES}${EXTRINSICS}
# python train.py --niter 100 $EXTRINSICS --save_epoch_freq 20 --display_env $NAME --rendererType $RENDERER_TYPE --batch_size $BATCH_SIZE --tex_dim $TEX_DIM --tex_features $TEX_FEATURES --dataroot $DATASETS_DIR/$DATA --name $NAME --num_depth_layers $NUM_DEPTH_LAYERS --renderer $RENDERER --model $MODEL --netG unet_256 --dataset_mode $DATASET_MODE --no_lsgan --norm batch --pool_size 0 --gpu_ids $GPU_ID --lr $LR --display_freq $DISP_FREQ --checkpoints_dir $CHECKPOINTS_DIR --print_freq $DISP_FREQ $CONTINUE $OPTIONS
# python test.py --rendererType $RENDERER_TYPE $EXTRINSICS --num_depth_layers $NUM_DEPTH_LAYERS --name $NAME --epoch $EPOCH --results_dir $RESULTS_DIR --checkpoints_dir $CHECKPOINTS_DIR --display_winsize 512 --tex_dim $TEX_DIM --tex_features $TEX_FEATURES --dataroot $DATASETS_DIR/$DATA  --lossType $LOSS --model $MODEL --netG unet_256 --dataset_mode $DATASET_MODE --norm batch --gpu_ids $GPU_ID $OPTIONS
# python test.py --rendererType $RENDERER_TYPE $EXTRINSICS --phase test_interpolating --num_depth_layers $NUM_DEPTH_LAYERS --name $NAME --epoch $EPOCH --results_dir $RESULTS_DIR --checkpoints_dir $CHECKPOINTS_DIR --display_winsize 512 --tex_dim $TEX_DIM --tex_features $TEX_FEATURES --dataroot $DATASETS_DIR/$DATA  --lossType $LOSS --model $MODEL --netG unet_256 --dataset_mode $DATASET_MODE --norm batch --gpu_ids $GPU_ID $OPTIONS
