function validateModalFormFields(form, fields) {
    var isValid = true;

    fields.forEach(function (field) {
        var inputElement = form.querySelector('#' + field.id);
        if (!inputElement) {
            console.warn('Input element with id "' + field.id + '" not found within form.');
            return;
        }

        var errorElement = form.querySelector('#' + field.id + 'Error');
        if (!errorElement) {
            errorElement = document.createElement('span');
            errorElement.id = field.id + 'Error';
            errorElement.className = 'text-danger';
            inputElement.parentNode.appendChild(errorElement);
        }

        if (inputElement.value.trim() === '') {
            isValid = false;
            inputElement.style.border = '1px solid red';
            errorElement.textContent = field.message;
        } else {
            inputElement.style.border = '';
            errorElement.textContent = '';
        }

        if (field.id === 'Password') {
            const passwordError = validatePassword(inputElement.value.trim());
            if (passwordError !== "") {
                isValid = false;
                inputElement.style.border = '1px solid red';
                errorElement.textContent = passwordError;
                return;
            }
        }
    });

    return isValid;
}

function validatePassword(password) {
    const minLength = 6;
    const hasNumber = /\d/;
    const hasUpperCase = /[A-Z]/;
    const hasLowerCase = /[a-z]/;
    const hasSpecialChar = /[!@@#$%^&*(),.?":{}|<>]/;

    if (password.length < minLength) {
        return "Mật khẩu phải có ít nhất 6 ký tự.";
    }
    if (!hasNumber.test(password)) {
        return "Mật khẩu phải chứa ít nhất 1 số.";
    }
    if (!hasUpperCase.test(password)) {
        return "Mật khẩu phải chứa ít nhất 1 chữ in hoa.";
    }
    if (!hasLowerCase.test(password)) {
        return "Mật khẩu phải chứa ít nhất 1 chữ thường.";
    }
    if (!hasSpecialChar.test(password)) {
        return "Mật khẩu phải chứa ít nhất 1 ký tự đặc biệt.";
    }
    return "";
}