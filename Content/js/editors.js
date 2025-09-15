"use strict";

!function (NioApp, $) {
  "use strict"; 
  
  // Store original modal method to prevent conflicts
  var originalModal = $.fn.modal;
  
  // Custom modal handler for Summernote
  var summernoteModalHandler = {
    show: function($modal) {
      // Remove any existing backdrops
      $('.modal-backdrop').remove();
      
      // Show modal without Bootstrap's modal handler
      $modal.css({
        'display': 'block',
        'z-index': 9999,
        'position': 'fixed',
        'top': '50%',
        'left': '50%',
        'transform': 'translate(-50%, -50%)',
        'background': 'white',
        'border': '1px solid #ccc',
        'border-radius': '5px',
        'box-shadow': '0 0 20px rgba(0,0,0,0.5)'
      });
      
      // Add a custom backdrop
      var backdrop = $('<div class="custom-modal-backdrop"></div>').css({
        'position': 'fixed',
        'top': 0,
        'left': 0,
        'width': '100%',
        'height': '100%',
        'background': 'rgba(0,0,0,0.5)',
        'z-index': 9998
      });
      
      $('body').append(backdrop);
      
      // Handle close
      backdrop.on('click', function() {
        summernoteModalHandler.hide($modal);
      });
      
      $modal.find('.close').on('click', function() {
        summernoteModalHandler.hide($modal);
      });
    },
    
    hide: function($modal) {
      $modal.css('display', 'none');
      $('.custom-modal-backdrop').remove();
    }
  };
  
  // Global initialization tracker
  NioApp.editorInitialized = false;
  
  // Single global Summernote initializer
  NioApp.SummerNote = function () {
    // Prevent multiple initializations
    if (NioApp.editorInitialized) return;
    
    // First, clean up any existing instances
    if ($.fn.summernote) {
      $('.summernote-minimal, .summernote-basic').each(function() {
        try {
          if ($(this).next('.note-editor').length) {
            $(this).summernote('destroy');
          }
        } catch (e) {
          // Ignore errors during cleanup
        }
      });
    }
    
    // Initialize basic summernote
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
            ],
            callbacks: {
              onInit: function() {
                // Override modal for this instance
                summernoteModalHandler.overrideForInstance($this);
              }
            }
          });
        } catch (e) {
          console.error('Error initializing basic summernote', e);
        }
      });
    }

    // Initialize minimal summernote with custom modal handling
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
                console.log('Summernote initialized for:', $this.attr('id'));
                
                // Add CSS fixes
                if (!$('#summernote-fixes').length) {
                  $('head').append(`
                    <style id="summernote-fixes">
                      .note-editable ul {
                        list-style-type: disc !important;
                        padding-left: 20px !important;
                        margin-left: 0 !important;
                      }
                      .note-editable ol {
                        list-style-type: decimal !important;
                        padding-left: 20px !important;
                        margin-left: 0 !important;
                      }
                      .note-editable li {
                        display: list-item !important;
                        margin-bottom: 5px !important;
                      }
                      
                      /* Custom modal styling */
                      .custom-modal-backdrop {
                        position: fixed;
                        top: 0;
                        left: 0;
                        width: 100%;
                        height: 100%;
                        background: rgba(0,0,0,0.5);
                        z-index: 9998;
                      }
                      
                      .note-modal {
                        z-index: 9999 !important;
                        position: fixed !important;
                      }
                      
                      /* Hide Bootstrap modal backdrops that conflict */
                      .modal-backdrop {
                        display: none !important;
                      }
                    </style>
                  `);
                }
                
                // Load initial content
                var $hiddenField = $this.closest('form').find('#PageTextHidden, #DetailsHidden');
                if ($hiddenField.length && $hiddenField.val()) {
                  $this.summernote('code', $hiddenField.val());
                }
                
                // Override modal handling to prevent conflicts
                setTimeout(function() {
                  // Find all note modals and override their behavior
                  $('.note-modal').each(function() {
                    var $modal = $(this);
                    
                    // Remove Bootstrap modal data to prevent conflicts
                    $modal.removeData('bs.modal');
                    
                    // Override the modal function for this element
                    $modal.modal = function(action) {
                      if (action === 'show') {
                        summernoteModalHandler.show($modal);
                      } else if (action === 'hide') {
                        summernoteModalHandler.hide($modal);
                      }
                      return $modal;
                    };
                  });
                  
                  // Override link, picture, video buttons to use custom modal
                  $this.next('.note-editor').find('.note-btn').each(function() {
                    var $btn = $(this);
                    var originalClick = $btn.data('original-click') || $btn.attr('onclick');
                    
                    if ($btn.hasClass('note-btn-link') || 
                        $btn.hasClass('note-btn-picture') || 
                        $btn.hasClass('note-btn-video') ||
                        $btn.attr('data-original-title') === 'Link' ||
                        $btn.attr('data-original-title') === 'Picture' ||
                        $btn.attr('data-original-title') === 'Video') {
                      
                      $btn.off('click').on('click', function(e) {
                        e.preventDefault();
                        e.stopPropagation();
                        
                        // Small delay to ensure any existing modals are handled
                        setTimeout(function() {
                          // Find the modal that should open
                          var modalSelector = '';
                          if ($btn.hasClass('note-btn-link') || $btn.attr('data-original-title') === 'Link') {
                            modalSelector = '.note-modal[data-modal="link"]';
                            if (!$(modalSelector).length) modalSelector = '.note-modal:contains("Insert Link")';
                          } else if ($btn.hasClass('note-btn-picture') || $btn.attr('data-original-title') === 'Picture') {
                            modalSelector = '.note-modal[data-modal="picture"]';
                            if (!$(modalSelector).length) modalSelector = '.note-modal:contains("Insert Image")';
                          } else if ($btn.hasClass('note-btn-video') || $btn.attr('data-original-title') === 'Video') {
                            modalSelector = '.note-modal[data-modal="video"]';
                            if (!$(modalSelector).length) modalSelector = '.note-modal:contains("Insert Video")';
                          }
                          
                          var $targetModal = $(modalSelector).first();
                          if ($targetModal.length) {
                            summernoteModalHandler.show($targetModal);
                          }
                        }, 10);
                      });
                    }
                  });
                  
                }, 100);
              }
            }
          });
          
          // Set up form submission handler
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
    
    // Mark as initialized
    NioApp.editorInitialized = true;
  }; 

  // Override global modal conflicts
  $(document).ready(function() {
    // Remove any conflicting event handlers
    $(document).off('focusin.modal');
    
    // Prevent Bootstrap modal conflicts
    if ($.fn.modal && originalModal) {
      $.fn.modal.Constructor.prototype.enforceFocus = function() {
        // Do nothing to prevent focus conflicts
      };
    }
  });

  // Tinymce Init @v1.0
  NioApp.Tinymce = function () {
    // Tinymce initialization remains the same
    var _tinymce_basic = '.tinymce-basic';
    var _tinymce_menubar = '.tinymce-menubar';
    var _tinymce_toolbar = '.tinymce-toolbar';
    var _tinymce_inline = '.tinymce-inline';

    if (typeof tinymce !== 'undefined') {
      if ($(_tinymce_basic).exists()) {
        tinymce.init({
          selector: _tinymce_basic,
          content_css: true,
          skin: false,
          branding: false
        });
      }

      if ($(_tinymce_menubar).exists()) {
        tinymce.init({
          selector: _tinymce_menubar,
          content_css: true,
          skin: false,
          branding: false,
          toolbar: false
        });
      }

      if ($(_tinymce_toolbar).exists()) {
        tinymce.init({
          selector: _tinymce_toolbar,
          content_css: true,
          skin: false,
          branding: false,
          menubar: false
        });
      }

      if ($(_tinymce_inline).exists()) {
        tinymce.init({
          selector: _tinymce_inline,
          content_css: false,
          skin: false,
          branding: false,
          menubar: false,
          inline: true,
          toolbar: ['undo redo | bold italic underline | fontselect fontsizeselect', 'forecolor backcolor | alignleft aligncenter alignright alignfull | numlist bullist outdent indent']
        });
      }
    }
  }; 

  // Quill Init @v1.0
  NioApp.Quill = function () {
    if (typeof Quill === 'undefined') return;
    
    var _basic = '.quill-basic';
    var _minimal = '.quill-minimal';
    
    function initQuill(selector, options) {
      if ($(selector).exists()) {
        $(selector).each(function() {
          if (!$(this).hasClass('quill-initialized')) {
            try {
              new Quill(this, options);
              $(this).addClass('quill-initialized');
            } catch (e) {
              console.error('Error initializing Quill', e);
            }
          }
        });
      }
    }
    
    initQuill(_basic, {
      modules: {
        toolbar: [
          ['bold', 'italic', 'underline', 'strike'],
          ['blockquote', 'code-block'],
          [{'list': 'ordered'}, {'list': 'bullet'}],
          [{'script': 'sub'}, {'script': 'super'}],
          [{'indent': '-1'}, {'indent': '+1'}],
          [{'header': [1, 2, 3, 4, 5, 6]}],
          [{'color': []}, {'background': []}],
          [{'font': []}],
          [{'align': []}],
          ['clean']
        ]
      },
      theme: 'snow'
    });
    
    initQuill(_minimal, {
      modules: {
        toolbar: [
          ['bold', 'italic', 'underline'],
          ['blockquote', {'list': 'bullet'}],
          [{'header': 1}, {'header': 2}, {'header': [3, 4, 5, 6, false]}],
          [{'align': []}],
          ['clean']
        ]
      },
      theme: 'snow'
    });
  };

  // Editor Init @v1
  NioApp.EditorInit = function () {
    NioApp.SummerNote();
    NioApp.Tinymce();
    NioApp.Quill();
  };

  // Add to document ready
  NioApp.coms.docReady.push(NioApp.EditorInit);
}(NioApp, jQuery);