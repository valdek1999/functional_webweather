this.dragAndDrop = document.querySelector('.drag-and-drop');
this.uploadBtn = document.getElementById('upload-btn');
this.uploadExplorer = document.getElementById('upload-file');
this.files = null;

this.uploadBtn.addEventListener('click', () => {
    this.uploadExplorer.click();
    console.log('Сlick on file explorer');
});
this.uploadExplorer.onchange = ((e) => {
    console.log('File has started loading to server');

    this.files = e.target.files;
});

this.dragAndDrop.addEventListener('dragenter', (e) => {
    e.preventDefault();
    this.dragAndDrop.classList.add('active')
    console.log('dragenter');
})
this.dragAndDrop.addEventListener('dragleave', (e) => {
    e.preventDefault();
    this.dragAndDrop.classList.remove('active')
    console.log('dragleave');
})
this.dragAndDrop.addEventListener('dragover', (e) => {
    e.preventDefault();
    console.log('dragover');
})

this.dragAndDrop.addEventListener('drop', (e) => {
    e.preventDefault();
    this.dragAndDrop.classList.remove('active')

    this.files = e.dataTransfer.files;
})

function readFile(file){
    return function () {
        console.log(file);
    }
}

function sendContract(){
    var formdata = new FormData(document.forms.addfiles);
    if (files == null) {
        alert(`Загрузите файлы перед отправкой`);
    }
    var i = 0, len = this.files.length, file;
    
    for (; i < len; i++) {
        file = this.files[i];
        formdata.append("uploads", file);
    }
    var xhr = new XMLHttpRequest();
    xhr.open("POST", "/Home/AddFile");

    setInterval(
        function send() {
            xhr.send(formdata);

            xhr.onload = function () {
                if (xhr.status != 200) { // анализируем HTTP-статус ответа, если статус не 200, то произошла ошибка
                    alert(`${xhr.responseText}.`); // Например, 404: Not Found
                } else { // если всё прошло гладко, выводим результат
                    alert(`Эксель файлы успешно загружены`); // response -- это ответ сервера
                }
            };
        }, 1000)
    alert(`Эксель файлы загружаются`);
}