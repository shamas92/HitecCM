/**
 * Image Crop Functionality
 * Provides interactive image cropping with drag and resize capabilities
 */

class ImageCropper {
    constructor() {
        this.cropImage = null;
        this.cropSelection = null;
        this.imageContainer = null;
        this.isDragging = false;
        this.isResizing = false;
        this.dragStart = { x: 0, y: 0 };
        this.resizeHandle = null;
        this.imageRect = null;
        this.aspectRatio = null;

        this.init();
    }

    init() {
        this.cropImage = document.getElementById('cropImage');
        this.cropSelection = document.getElementById('cropSelection');
        this.imageContainer = document.querySelector('.image-container');

        this.bindEvents();
    }

    bindEvents() {
        // Modal events
        $('#cropModal').on('shown.bs.modal', () => {
            this.setupCropArea();
        });

        // Control inputs
        $('#cropWidth, #cropHeight, #cropX, #cropY').on('input', () => {
            this.updateCropFromInputs();
        });

        $('#aspectRatio').on('change', () => {
            this.setAspectRatio();
        });

        // Action buttons
        $('#applyCrop').on('click', () => {
            this.applyCrop();
        });

        $('#resetCrop').on('click', () => {
            this.resetCrop();
        });

        // Mouse events for dragging and resizing
        this.setupMouseEvents();
    }

    setupMouseEvents() {
        // Crop selection dragging
        $(this.cropSelection).on('mousedown', (e) => {
            if (!$(e.target).hasClass('crop-handle')) {
                this.startDrag(e);
            }
        });

        // Handle resizing
        $('.crop-handle').on('mousedown', (e) => {
            this.startResize(e);
        });

        // Global mouse events
        $(document).on('mousemove', (e) => {
            if (this.isDragging) {
                this.drag(e);
            } else if (this.isResizing) {
                this.resize(e);
            }
        });

        $(document).on('mouseup', () => {
            this.stopDrag();
            this.stopResize();
        });
    }

    setupCropArea() {
        const imagePreview = document.getElementById('imagePreview');
        if (imagePreview && imagePreview.src && imagePreview.src !== '#') {
            this.cropImage.src = imagePreview.src;

            // Wait for image to load
            this.cropImage.onload = () => {
                this.imageRect = this.cropImage.getBoundingClientRect();
                this.resetCrop();
            };
        }
    }

    resetCrop() {
        if (!this.cropImage || !this.imageRect) return;

        const containerRect = this.imageContainer.getBoundingClientRect();
        const imageRect = this.cropImage.getBoundingClientRect();

        // Calculate relative position within container
        const relativeLeft = imageRect.left - containerRect.left;
        const relativeTop = imageRect.top - containerRect.top;

        // Set initial crop area (center 50% of image)
        const cropWidth = imageRect.width * 0.5;
        const cropHeight = imageRect.height * 0.5;
        const cropX = relativeLeft + (imageRect.width - cropWidth) / 2;
        const cropY = relativeTop + (imageRect.height - cropHeight) / 2;

        this.setCropArea(cropX, cropY, cropWidth, cropHeight);
        this.updateInputs();
    }

    setCropArea(x, y, width, height) {
        if (!this.cropSelection) return;

        this.cropSelection.style.left = x + 'px';
        this.cropSelection.style.top = y + 'px';
        this.cropSelection.style.width = width + 'px';
        this.cropSelection.style.height = height + 'px';
    }

    updateInputs() {
        const rect = this.cropSelection.getBoundingClientRect();
        const containerRect = this.imageContainer.getBoundingClientRect();
        const imageRect = this.cropImage.getBoundingClientRect();

        // Calculate relative position to image
        const relativeX = rect.left - imageRect.left;
        const relativeY = rect.top - imageRect.top;

        $('#cropX').val(Math.round(relativeX));
        $('#cropY').val(Math.round(relativeY));
        $('#cropWidth').val(Math.round(rect.width));
        $('#cropHeight').val(Math.round(rect.height));
    }

    updateCropFromInputs() {
        const x = parseInt($('#cropX').val()) || 0;
        const y = parseInt($('#cropY').val()) || 0;
        const width = parseInt($('#cropWidth').val()) || 100;
        const height = parseInt($('#cropHeight').val()) || 100;

        const containerRect = this.imageContainer.getBoundingClientRect();
        const imageRect = this.cropImage.getBoundingClientRect();

        // Convert image-relative coordinates to container-relative
        const containerX = (imageRect.left - containerRect.left) + x;
        const containerY = (imageRect.top - containerRect.top) + y;

        this.setCropArea(containerX, containerY, width, height);
    }

    setAspectRatio() {
        const ratio = $('#aspectRatio').val();

        if (ratio === 'free') {
            this.aspectRatio = null;
            return;
        }

        const [w, h] = ratio.split(':').map(Number);
        this.aspectRatio = w / h;

        // Adjust current crop to match aspect ratio
        this.adjustToAspectRatio();
    }

    adjustToAspectRatio() {
        if (!this.aspectRatio) return;

        const rect = this.cropSelection.getBoundingClientRect();
        const currentWidth = rect.width;
        const currentHeight = rect.height;

        let newWidth, newHeight;

        if (currentWidth / currentHeight > this.aspectRatio) {
            // Too wide, adjust width
            newHeight = currentHeight;
            newWidth = newHeight * this.aspectRatio;
        } else {
            // Too tall, adjust height
            newWidth = currentWidth;
            newHeight = newWidth / this.aspectRatio;
        }

        const containerRect = this.imageContainer.getBoundingClientRect();
        const x = rect.left - containerRect.left;
        const y = rect.top - containerRect.top;

        this.setCropArea(x, y, newWidth, newHeight);
        this.updateInputs();
    }

    startDrag(e) {
        this.isDragging = true;
        const rect = this.cropSelection.getBoundingClientRect();
        this.dragStart = {
            x: e.clientX - rect.left,
            y: e.clientY - rect.top
        };
        e.preventDefault();
    }

    drag(e) {
        if (!this.isDragging) return;

        const containerRect = this.imageContainer.getBoundingClientRect();
        const imageRect = this.cropImage.getBoundingClientRect();
        const cropRect = this.cropSelection.getBoundingClientRect();

        let newX = e.clientX - containerRect.left - this.dragStart.x;
        let newY = e.clientY - containerRect.top - this.dragStart.y;

        // Constrain to image bounds
        const minX = imageRect.left - containerRect.left;
        const minY = imageRect.top - containerRect.top;
        const maxX = imageRect.right - containerRect.left - cropRect.width;
        const maxY = imageRect.bottom - containerRect.top - cropRect.height;

        newX = Math.max(minX, Math.min(maxX, newX));
        newY = Math.max(minY, Math.min(maxY, newY));

        this.setCropArea(newX, newY, cropRect.width, cropRect.height);
        this.updateInputs();
    }

    stopDrag() {
        this.isDragging = false;
    }

    startResize(e) {
        this.isResizing = true;
        this.resizeHandle = e.target.className.split(' ').find(cls => cls.startsWith('crop-handle-'));
        e.preventDefault();
        e.stopPropagation();
    }

    resize(e) {
        if (!this.isResizing || !this.resizeHandle) return;

        const containerRect = this.imageContainer.getBoundingClientRect();
        const imageRect = this.cropImage.getBoundingClientRect();
        const cropRect = this.cropSelection.getBoundingClientRect();

        const mouseX = e.clientX - containerRect.left;
        const mouseY = e.clientY - containerRect.top;

        let newX = cropRect.left - containerRect.left;
        let newY = cropRect.top - containerRect.top;
        let newWidth = cropRect.width;
        let newHeight = cropRect.height;

        // Handle different resize directions
        switch (this.resizeHandle) {
            case 'crop-handle-nw':
                newWidth = (cropRect.right - containerRect.left) - mouseX;
                newHeight = (cropRect.bottom - containerRect.top) - mouseY;
                newX = mouseX;
                newY = mouseY;
                break;
            case 'crop-handle-ne':
                newWidth = mouseX - (cropRect.left - containerRect.left);
                newHeight = (cropRect.bottom - containerRect.top) - mouseY;
                newY = mouseY;
                break;
            case 'crop-handle-sw':
                newWidth = (cropRect.right - containerRect.left) - mouseX;
                newHeight = mouseY - (cropRect.top - containerRect.top);
                newX = mouseX;
                break;
            case 'crop-handle-se':
                newWidth = mouseX - (cropRect.left - containerRect.left);
                newHeight = mouseY - (cropRect.top - containerRect.top);
                break;
            case 'crop-handle-n':
                newHeight = (cropRect.bottom - containerRect.top) - mouseY;
                newY = mouseY;
                break;
            case 'crop-handle-s':
                newHeight = mouseY - (cropRect.top - containerRect.top);
                break;
            case 'crop-handle-w':
                newWidth = (cropRect.right - containerRect.left) - mouseX;
                newX = mouseX;
                break;
            case 'crop-handle-e':
                newWidth = mouseX - (cropRect.left - containerRect.left);
                break;
        }

        // Apply aspect ratio constraint if set
        if (this.aspectRatio && ['crop-handle-nw', 'crop-handle-ne', 'crop-handle-sw', 'crop-handle-se'].includes(this.resizeHandle)) {
            if (newWidth / newHeight > this.aspectRatio) {
                newWidth = newHeight * this.aspectRatio;
            } else {
                newHeight = newWidth / this.aspectRatio;
            }

            // Adjust position for corner handles
            if (this.resizeHandle === 'crop-handle-nw') {
                newX = (cropRect.right - containerRect.left) - newWidth;
                newY = (cropRect.bottom - containerRect.top) - newHeight;
            } else if (this.resizeHandle === 'crop-handle-ne') {
                newY = (cropRect.bottom - containerRect.top) - newHeight;
            } else if (this.resizeHandle === 'crop-handle-sw') {
                newX = (cropRect.right - containerRect.left) - newWidth;
            }
        }

        // Ensure minimum size
        newWidth = Math.max(50, newWidth);
        newHeight = Math.max(50, newHeight);

        // Constrain to image bounds
        const imageLeft = imageRect.left - containerRect.left;
        const imageTop = imageRect.top - containerRect.top;
        const imageRight = imageRect.right - containerRect.left;
        const imageBottom = imageRect.bottom - containerRect.top;

        newX = Math.max(imageLeft, Math.min(imageRight - newWidth, newX));
        newY = Math.max(imageTop, Math.min(imageBottom - newHeight, newY));
        newWidth = Math.min(newWidth, imageRight - newX);
        newHeight = Math.min(newHeight, imageBottom - newY);

        this.setCropArea(newX, newY, newWidth, newHeight);
        this.updateInputs();
    }

    stopResize() {
        this.isResizing = false;
        this.resizeHandle = null;
    }

    applyCrop() {
        const canvas = document.createElement('canvas');
        const ctx = canvas.getContext('2d');

        // Get crop dimensions relative to the actual image
        const imageRect = this.cropImage.getBoundingClientRect();
        const cropRect = this.cropSelection.getBoundingClientRect();

        const scaleX = this.cropImage.naturalWidth / imageRect.width;
        const scaleY = this.cropImage.naturalHeight / imageRect.height;

        const cropX = (cropRect.left - imageRect.left) * scaleX;
        const cropY = (cropRect.top - imageRect.top) * scaleY;
        const cropWidth = cropRect.width * scaleX;
        const cropHeight = cropRect.height * scaleY;

        canvas.width = cropWidth;
        canvas.height = cropHeight;

        // Draw the cropped portion
        ctx.drawImage(
            this.cropImage,
            cropX, cropY, cropWidth, cropHeight,
            0, 0, cropWidth, cropHeight
        );

        // Convert to base64 and store
        const croppedDataUrl = canvas.toDataURL('image/jpeg', 0.9);
        $('#croppedImageData').val(croppedDataUrl);

        // Update preview
        $('#imagePreview').attr('src', croppedDataUrl);

        // Show success message
        if (typeof NioApp !== 'undefined' && NioApp.Toast) {
            NioApp.Toast('Image cropped successfully!', 'success');
        }

        // **MODIFICATION**: Close the Bootstrap modal
        $('#cropModal').modal('hide');

        // **FIX**: Manually remove the backdrop and clean up the body class
        $('.modal-backdrop').remove();
        $('body').removeClass('modal-open');
    }
}

// Initialize when document is ready
$(document).ready(function () {
    window.imageCropper = new ImageCropper();
});
