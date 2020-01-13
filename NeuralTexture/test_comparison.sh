set -ex

DATASETS_DIR=/mnt/raid/patrickradner/datasets
RESULTS_DIR=/mnt/raid/patrickradner/cmp_arch/results
CHECKPOINTS_DIR=/mnt/raid/patrickradner/cmp_arch/checkpoints
source "./experiment_setups/model_comparison/L1_tex256x8_lab3.sh"

 #renderer
#RENDERER=MultiTarget-neuralRenderer_200
RENDERER=no_renderer

 #optimizer parameters
LR=0.001
BATCH_SIZE=1

 #GPU
GPU_ID="0"

 #display params
DISP_FREQ=50
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

python test.py --rendererType $RENDERER_TYPE --num_depth_layers $NUM_DEPTH_LAYERS --name $NAME --epoch $EPOCH --checkpoints_dir $CHECKPOINTS_DIR --display_winsize 512 --tex_dim $TEX_DIM --tex_features $TEX_FEATURES --dataroot $DATASETS_DIR/$DATA  --lossType $LOSS --model $MODEL --netG unet_256 --dataset_mode $DATASET_MODE --norm batch --gpu_ids $GPU_ID $OPTIONS


