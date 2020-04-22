set -ex

CONTINUE=
OPTIONS=

DATASETS_DIR=/mnt/raid/patrickradner/datasets
RESULTS_DIR=/mnt/raid/patrickradner/cmp_extrinsics/results
CHECKPOINTS_DIR=/mnt/raid/patrickradner/cmp_extrinsics/checkpoints

# optimizer parameters
LR=0.001
BATCH_SIZE=1
# renderer
#RENDERER=MultiTarget-neuralRenderer_200
RENDERER=no_renderer
# GPU
GPU_ID="2"

# display params
DISP_FREQ=100
LOSS=L1
EPOCH=latest



if [[ $(nvidia-smi | grep "^|    $GPU_ID    ") ]]; then
    read -p "GPU currently in use, continue? " -n 1 -r
    echo
    if [[ $REPLY =~ ^[Nn]$ ]]
    then
        exit 1
    fi
fi

source "./experiment_setups/extrinsics_cmp/PerPixel2_lab3_noex.sh"
# python train.py --niter 100 $EXTRINSICS --save_epoch_freq 20 --display_env $NAME --rendererType $RENDERER_TYPE --batch_size $BATCH_SIZE --tex_dim $TEX_DIM --tex_features $TEX_FEATURES --dataroot $DATASETS_DIR/$DATA --name $NAME --num_depth_layers $NUM_DEPTH_LAYERS --renderer $RENDERER --model $MODEL --netG unet_256 --dataset_mode $DATASET_MODE --no_lsgan --norm batch --pool_size 0 --gpu_ids $GPU_ID --lr $LR --display_freq $DISP_FREQ --checkpoints_dir $CHECKPOINTS_DIR --print_freq $DISP_FREQ $CONTINUE $OPTIONS
python test.py --rendererType $RENDERER_TYPE $EXTRINSICS --num_depth_layers $NUM_DEPTH_LAYERS --name $NAME --epoch $EPOCH --results_dir $RESULTS_DIR --checkpoints_dir $CHECKPOINTS_DIR --display_winsize 512 --tex_dim $TEX_DIM --tex_features $TEX_FEATURES --dataroot $DATASETS_DIR/$DATA  --lossType $LOSS --model $MODEL --netG unet_256 --dataset_mode $DATASET_MODE --norm batch --gpu_ids $GPU_ID $OPTIONS &
python test.py --rendererType $RENDERER_TYPE $EXTRINSICS --phase test_interpolating --num_depth_layers $NUM_DEPTH_LAYERS --name $NAME --epoch $EPOCH --results_dir $RESULTS_DIR --checkpoints_dir $CHECKPOINTS_DIR --display_winsize 512 --tex_dim $TEX_DIM --tex_features $TEX_FEATURES --dataroot $DATASETS_DIR/$DATA  --lossType $LOSS --model $MODEL --netG unet_256 --dataset_mode $DATASET_MODE --norm batch --gpu_ids $GPU_ID $OPTIONS &

GPU_ID="3"

source "./experiment_setups/extrinsics_cmp/PerPixel2_lab3_ex.sh"
# python train.py --niter 100 $EXTRINSICS --save_epoch_freq 20 --display_env $NAME --rendererType $RENDERER_TYPE --batch_size $BATCH_SIZE --tex_dim $TEX_DIM --tex_features $TEX_FEATURES --dataroot $DATASETS_DIR/$DATA --name $NAME --num_depth_layers $NUM_DEPTH_LAYERS --renderer $RENDERER --model $MODEL --netG unet_256 --dataset_mode $DATASET_MODE --no_lsgan --norm batch --pool_size 0 --gpu_ids $GPU_ID --lr $LR --display_freq $DISP_FREQ --checkpoints_dir $CHECKPOINTS_DIR --print_freq $DISP_FREQ $CONTINUE $OPTIONS
python test.py --rendererType $RENDERER_TYPE $EXTRINSICS --num_depth_layers $NUM_DEPTH_LAYERS --name $NAME --epoch $EPOCH --results_dir $RESULTS_DIR --checkpoints_dir $CHECKPOINTS_DIR --display_winsize 512 --tex_dim $TEX_DIM --tex_features $TEX_FEATURES --dataroot $DATASETS_DIR/$DATA  --lossType $LOSS --model $MODEL --netG unet_256 --dataset_mode $DATASET_MODE --norm batch --gpu_ids $GPU_ID $OPTIONS &
python test.py --rendererType $RENDERER_TYPE $EXTRINSICS --phase test_interpolating --num_depth_layers $NUM_DEPTH_LAYERS --name $NAME --epoch $EPOCH --results_dir $RESULTS_DIR --checkpoints_dir $CHECKPOINTS_DIR --display_winsize 512 --tex_dim $TEX_DIM --tex_features $TEX_FEATURES --dataroot $DATASETS_DIR/$DATA  --lossType $LOSS --model $MODEL --netG unet_256 --dataset_mode $DATASET_MODE --norm batch --gpu_ids $GPU_ID $OPTIONS &


source "./experiment_setups/extrinsics_cmp/PerPixel2_lab3_sh.sh"
#python train.py --niter 100 $EXTRINSICS --save_epoch_freq 20 --display_env $NAME --rendererType $RENDERER_TYPE --batch_size $BATCH_SIZE --tex_dim $TEX_DIM --tex_features $TEX_FEATURES --dataroot $DATASETS_DIR/$DATA --name $NAME --num_depth_layers $NUM_DEPTH_LAYERS --renderer $RENDERER --model $MODEL --netG unet_256 --dataset_mode $DATASET_MODE --no_lsgan --norm batch --pool_size 0 --gpu_ids $GPU_ID --lr $LR --display_freq $DISP_FREQ --checkpoints_dir $CHECKPOINTS_DIR --print_freq $DISP_FREQ $CONTINUE $OPTIONS
python test.py --rendererType $RENDERER_TYPE $EXTRINSICS --num_depth_layers $NUM_DEPTH_LAYERS --name $NAME --epoch $EPOCH --results_dir $RESULTS_DIR --checkpoints_dir $CHECKPOINTS_DIR --display_winsize 512 --tex_dim $TEX_DIM --tex_features $TEX_FEATURES --dataroot $DATASETS_DIR/$DATA  --lossType $LOSS --model $MODEL --netG unet_256 --dataset_mode $DATASET_MODE --norm batch --gpu_ids $GPU_ID $OPTIONS &
python test.py --rendererType $RENDERER_TYPE $EXTRINSICS --phase test_interpolating --num_depth_layers $NUM_DEPTH_LAYERS --name $NAME --epoch $EPOCH --results_dir $RESULTS_DIR --checkpoints_dir $CHECKPOINTS_DIR --display_winsize 512 --tex_dim $TEX_DIM --tex_features $TEX_FEATURES --dataroot $DATASETS_DIR/$DATA  --lossType $LOSS --model $MODEL --netG unet_256 --dataset_mode $DATASET_MODE --norm batch --gpu_ids $GPU_ID $OPTIONS &
