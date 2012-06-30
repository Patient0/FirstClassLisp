; We do this to work around the reader - need to get
; '.' as a symbol
(define-macro define-dot (definition)
    `(,define ,(string->symbol ".") ,definition))
(define-dot (macro
    (lambda (method instance . args)
        `(,invoke-instance ,(symbol->string method) ,instance ,@args))))
