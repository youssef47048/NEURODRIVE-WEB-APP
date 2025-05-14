// Wait for the DOM to be fully loaded
document.addEventListener('DOMContentLoaded', function () {
    // Password toggle functionality
    const togglePasswordButtons = document.querySelectorAll('.toggle-password');
    togglePasswordButtons.forEach(button => {
        button.addEventListener('click', function(e) {
            e.preventDefault();
            const formFloating = this.closest('.form-floating');
            const passwordField = formFloating.querySelector('input[type="password"], input[type="text"]');
            const icon = this.querySelector('i');
            
            if (passwordField.type === 'password') {
                passwordField.type = 'text';
                icon.classList.remove('bi-eye-slash');
                icon.classList.add('bi-eye');
            } else {
                passwordField.type = 'password';
                icon.classList.remove('bi-eye');
                icon.classList.add('bi-eye-slash');
            }
        });
    });

    // Debug function to directly submit form
    window.submitVehicleForm = function() {
        console.log('Manual form submission triggered');
        document.getElementById('vehicle-form').submit();
    };

    // Handle face image upload
    const fileInput = document.getElementById('face-image-input');
    const previewImage = document.getElementById('preview-image');
    const base64Input = document.getElementById('FaceImageBase64');
    
    if (fileInput && previewImage && base64Input) {
        fileInput.addEventListener('change', function (e) {
            const file = e.target.files[0];
            if (file) {
                const reader = new FileReader();
                
                reader.onload = function (e) {
                    // Display the image
                    previewImage.src = e.target.result;
                    previewImage.style.display = 'block';
                    
                    // Store the base64 string (strip the data:image/xxx;base64, part)
                    const base64String = e.target.result.split(',')[1];
                    base64Input.value = base64String;
                };
                
                reader.readAsDataURL(file);
            }
        });
    }
    
    // Add vehicle form validation
    const vehicleForm = document.getElementById('vehicle-form');
    if (vehicleForm) {
        vehicleForm.addEventListener('submit', function (e) {
            const carIdInput = document.getElementById('CarId');
            const nameInput = document.getElementById('Name');
            let isValid = true;
            
            if (!carIdInput.value.trim()) {
                e.preventDefault();
                isValid = false;
                alert('Car ID is required');
                carIdInput.classList.add('is-invalid');
            } else {
                carIdInput.classList.remove('is-invalid');
            }
            
            if (!nameInput.value.trim()) {
                e.preventDefault();
                isValid = false;
                alert('Vehicle name is required');
                nameInput.classList.add('is-invalid');
            } else {
                nameInput.classList.remove('is-invalid');
            }
            
            // Debug feedback
            if (isValid) {
                console.log('Form validation passed - submitting form...');
            } else {
                console.log('Form validation failed - submission prevented');
            }
        });
    }
    
    // Add driver form validation
    const driverForm = document.getElementById('driver-form');
    if (driverForm) {
        driverForm.addEventListener('submit', function (e) {
            const driverNameInput = document.getElementById('DriverName');
            const faceImageInput = document.getElementById('FaceImageBase64');
            let isValid = true;
            
            if (!driverNameInput.value.trim()) {
                e.preventDefault();
                isValid = false;
                alert('Driver name is required');
                driverNameInput.classList.add('is-invalid');
            } else {
                driverNameInput.classList.remove('is-invalid');
            }
            
            if (!faceImageInput.value.trim()) {
                e.preventDefault();
                isValid = false;
                alert('Face image is required');
            }
            
            // Debug feedback
            if (isValid) {
                console.log('Driver form validation passed - submitting form...');
            } else {
                console.log('Driver form validation failed - submission prevented');
            }
        });
    }
}); 