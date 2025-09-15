"use strict";

!function (NioApp, $) {
  "use strict"; 
  
  var originalModal = $.fn.modal; // retained in case needed
  
  NioApp.editorInitialized = false;
  
  NioApp.SummerNote = function () {
    if (NioApp.editorInitialized) return;
    
    if ($.fn.summernote) {
      $('.summernote-minimal, .summernote-basic').each(function() {
        try {
          if ($(this).next('.note-editor').length) {
            $(this).summernote('destroy');
          }
        } catch (e) {}
      });
    }
    
    var _basic = '.summernote-basic';
    if ($(_basic).exists() && $.fn.summernote) {
      $(_basic).each(function () {
        var $this = $(this);
        if (!$this.attr('id')) {
          $this.attr('id', 'summernote-basic-' + Math.floor(Math.random() * 100000));
        }
        try {
          $this.summernote({
            placeholder: 'Hello stand alone ui',
            tabsize: 2,
            height: 120,
            dialogsInBody: true,
            toolbar: [
              ['style', ['style']], 
              ['font', ['bold', 'underline', 'strikethrough', 'clear']], 
              ['font', ['superscript', 'subscript']], 
              ['color', ['color']], 
              ['fontsize', ['fontsize', 'height']], 
              ['para', ['ul', 'ol', 'paragraph']], 
              ['table', ['table']], 
              ['insert', ['link']], 
              ['view', ['fullscreen', 'codeview', 'help']]
            ]
          });
        } catch (e) {
          console.error('Error initializing basic summernote', e);
        }
      });
    }

    var _minimal = '.summernote-minimal';
    if ($(_minimal).exists() && $.fn.summernote) {
      $(_minimal).each(function () {
        var $this = $(this);
        if (!$this.attr('id')) {
          $this.attr('id', 'summernote-minimal-' + Math.floor(Math.random() * 100000));
        }
        try {
          $this.summernote({
            placeholder: 'Enter details here...',
            tabsize: 2,
            height: 300,
            dialogsInBody: true,
            fontNames: ['Arial', 'Arial Black', 'Calibri', 'Times New Roman', 'Courier New', 'Verdana', 'Montserrat', 'Poppins'],
            fontNamesIgnoreCheck: ['Montserrat', 'Poppins'],
            fontSizes: ['8', '9', '10', '11', '12', '14', '16', '18', '24', '36', '48', '64'],
            toolbar: [
              ['style', ['style']],
              ['font', ['bold', 'italic', 'underline', 'strikethrough', 'superscript', 'subscript', 'clear']],
              ['fontname', ['fontname']],
              ['fontsize', ['fontsize']],
              ['color', ['color']],
              ['para', ['ul', 'ol', 'paragraph']],
              ['height', ['height']],
              ['table', ['table']],
              ['insert', ['link', 'picture', 'video']],
              ['view', ['fullscreen', 'codeview', 'help']]
            ],
            callbacks: {
              onInit: function() {
                if (!$('#summernote-fixes').length) {
                  $('head').append(`
                    <style id="summernote-fixes">
                      .note-editable ul { list-style-type: disc !important; padding-left: 20px !important; margin-left: 0 !important; }
                      .note-editable ol { list-style-type: decimal !important; padding-left: 20px !important; margin-left: 0 !important; }
                      .note-editable li { display: list-item !important; margin-bottom: 5px !important; }
                      .note-modal.modal { z-index: 1055 !important; transition: none !important; animation: none !important; }
                      .modal-backdrop { z-index: 1050; }
                    </style>
                  `);
                }
                var $hiddenField = $this.closest('form').find('#PageTextHidden, #DetailsHidden');
                if ($hiddenField.length && $hiddenField.val()) {
                  $this.summernote('code', $hiddenField.val());
                }
              }
            }
          });
          $this.closest('form').off('submit.summernote').on('submit.summernote', function() {
            var content = $this.summernote('code');
            var $hiddenField = $(this).find('#PageTextHidden, #DetailsHidden');
            if ($hiddenField.length) {
              $hiddenField.val(content);
            }
          });
        } catch (e) {
          console.error('Error initializing minimal summernote', e);
        }
      });
    }
    
    NioApp.editorInitialized = true;
  }; 

  // Do NOT override enforceFocus now; keep Bootstrap default to stabilize focus handling
  $(document).ready(function() {
    // no-op
  });

  NioApp.Tinymce = function () {
    var _tinymce_basic = '.tinymce-basic';
    var _tinymce_menubar = '.tinymce-menubar';
    var _tinymce_toolbar = '.tinymce-toolbar';
    var _tinymce_inline = '.tinymce-inline';

    if (typeof tinymce !== 'undefined') {
      if ($(_tinymce_basic).exists()) {
        tinymce.init({ selector: _tinymce_basic, content_css: true, skin: false, branding: false });
      }
      if ($(_tinymce_menubar).exists()) {
        tinymce.init({ selector: _tinymce_menubar, content_css: true, skin: false, branding: false, toolbar: false });
      }
      if ($(_tinymce_toolbar).exists()) {
        tinymce.init({ selector: _tinymce_toolbar, content_css: true, skin: false, branding: false, menubar: false });
      }
      if ($(_tinymce_inline).exists()) {
        tinymce.init({ selector: _tinymce_inline, content_css: false, skin: false, branding: false, menubar: false, inline: true, toolbar: ['undo redo | bold italic underline | fontselect fontsizeselect', 'forecolor backcolor | alignleft aligncenter alignright alignfull | numlist bullist outdent indent'] });
      }
    }
  }; 

  NioApp.Quill = function () {
    if (typeof Quill === 'undefined') return;
    var _basic = '.quill-basic';
    var _minimal = '.quill-minimal';
    function initQuill(selector, options) {
      if ($(selector).exists()) {
        $(selector).each(function() {
          if (!$(this).hasClass('quill-initialized')) {
            try { new Quill(this, options); $(this).addClass('quill-initialized'); } catch (e) { console.error('Error initializing Quill', e); }
          }
        });
      }
    }
    initQuill(_basic, { modules: { toolbar: [ ['bold', 'italic', 'underline', 'strike'], ['blockquote', 'code-block'], [{'list': 'ordered'}, {'list': 'bullet'}], [{'script': 'sub'}, {'script': 'super'}], [{'indent': '-1'}, {'indent': '+1'}], [{'header': [1, 2, 3, 4, 5, 6]}], [{'color': []}, {'background': []}], [{'font': []}], [{'align': []}], ['clean'] ] }, theme: 'snow' });
    initQuill(_minimal, { modules: { toolbar: [ ['bold', 'italic', 'underline'], ['blockquote', {'list': 'bullet'}], [{'header': 1}, {'header': 2}, {'header': [3, 4, 5, 6, false]}], [{'align': []}], ['clean'] ] }, theme: 'snow' });
  };

  NioApp.EditorInit = function () {
    NioApp.SummerNote();
    NioApp.Tinymce();
    NioApp.Quill();
  };

  NioApp.coms.docReady.push(NioApp.EditorInit);
}(NioApp, jQuery);