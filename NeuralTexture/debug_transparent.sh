set -ex

CONTINUE=
OPTIONS=

DATASETS_DIR=/mnt/raid/patrickradner/datasets

# optimizer parameters
LR=0.0001
BATCH_SIZE=1
# renderer
#RENDERER=MultiTarget-neuralRenderer_200
RENDERER=no_renderer
# GPU
GPU_ID="1"

# display params
DISP_FREQ=1
LOSS=L1

EPOCH=latest

#source "./experiment_setups/GruPerPixel_lab3.sh"
#source "./experiment_setups/PerPixel_lab3.sh"
#source "./experiment_setups/PerPixel_lab3_vgg.sh"
#source "./experiment_setups/PerPixel2_lab3_vgg.sh"

source "./experiment_setups/PerPixel2_lab3_gan_small.sh"
#source "./experiment_setups/PerPixel2_lab3_gan_small.sh"
#source "./experiment_setups/Blend_lab3.sh"
#source "./experiment_setups/LstmPerPixel_4_4_lab3.sh"
#source "./experiment_setups/Lstm2UNET3_lab2.sh"
#source "./experiment_setups/PerLayerPerPixel4_lab2.sh"
#source "./experiment_setups/LstmPerPixel4_lab_2.sh"
#source "./experiment_setups/PerPixel4_lab_2.sh"
#source "./experiment_setups/UNET_5_lab_2.sh"
#source "./experiment_setups/Blend_lab_2.sh"
#source "./experiment_setups/Debug.sh"

#source "./experiment_setups/PerPixel2_tree_vgg.sh"
#source "./experiment_setups/UNET_5_tree.sh"
#source "./experiment_setups/UNET_8_tree.sh"
#source "./experiment_setups/PerPixel_fencecar.sh"
#source "./experiment_setups/UNET_5_fencecar.sh"


if [[ $(nvidia-smi | grep "^|    $GPU_ID    ") ]]; then
    read -p "GPU currently in use, continue? " -n 1 -r
    echo
    if [[ $REPLY =~ ^[Nn]$ ]]
    then
        exit 1
    fi
fi

python train.py --niter 100 --save_epoch_freq 20 --display_env $NAME --rendererType $RENDERER_TYPE --batch_size $BATCH_SIZE --tex_dim $TEX_DIM --tex_features $TEX_FEATURES --dataroot $DATASETS_DIR/$DATA --name $NAME --num_depth_layers $NUM_DEPTH_LAYERS --renderer $RENDERER --model $MODEL --netG unet_256 --dataset_mode $DATASET_MODE --no_lsgan --norm batch --pool_size 0 --gpu_ids $GPU_ID --lr $LR --display_freq $DISP_FREQ --print_freq $DISP_FREQ $CONTINUE $OPTIONS
#python test.py --rendererType $RENDERER_TYPE --phase test --num_depth_layers $NUM_DEPTH_LAYERS --name $NAME --epoch $EPOCH --display_winsize 512 --tex_dim $TEX_DIM --tex_features $TEX_FEATURES --dataroot $DATASETS_DIR/$DATA  --lossType $LOSS --model $MODEL --netG unet_256 --dataset_mode $DATASET_MODE --norm batch --gpu_ids $GPU_ID $OPTIONS
#python test.py --rendererType $RENDERER_TYPE --phase test_interpolating --num_depth_layers $NUM_DEPTH_LAYERS --name $NAME --epoch $EPOCH --display_winsize 512 --tex_dim $TEX_DIM --tex_features $TEX_FEATURES --dataroot $DATASETS_DIR/$DATA  --lossType $LOSS --model $MODEL --netG unet_256 --dataset_mode $DATASET_MODE --norm batch --gpu_ids $GPU_ID $OPTIONS
#python test.py --rendererType $RENDERER_TYPE --phase train --id_mapping "0,1,2,3,4,5" --num_depth_layers $NUM_DEPTH_LAYERS --name $NAME --epoch $EPOCH --display_winsize 512 --tex_dim $TEX_DIM --tex_features $TEX_FEATURES --dataroot $DATASETS_DIR/$DATA  --lossType $LOSS --model $MODEL --netG unet_256 --dataset_mode $DATASET_MODE --norm batch --gpu_ids $GPU_ID $OPTIONS

#--id_mapping "0,2,3,4,5,-1"