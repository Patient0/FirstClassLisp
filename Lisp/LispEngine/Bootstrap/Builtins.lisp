(define nil '())
(define list (lambda x x))
(define car (lambda ((a . b)) a))
(define cdr (lambda ((c . d)) d))
(define nil? (lambda (()) #t _ #f))
(define pair? (lambda ((_ . _)) #t _ #f))

; Now, let's implement simple non-nested quasiquote in terms of Lisp itself
; We need it quite early because writing macros without quasiquote
; is extremely painful!
; Using the builtin pattern matching of our lambda primitive makes
; this significantly simpler to implement.
(define expand-quasiquote
    (lambda
        (('unquote e))
            e
        ((('unquote-splicing x) . y))
            (list append x (expand-quasiquote y))
        ((x . y))
            (list cons (expand-quasiquote x) (expand-quasiquote y))
        x
            (cons quote x)))
(define quasiquote
    (macro expand-quasiquote))

; We use define followed by macro and a lambda an awful lot - so 
; let's define a macro to ease the overhead in
; writing macros!
(define define-macro
    (macro (lambda (name args . exprs)
            `(,define ,name
                (,macro (,lambda ,args (,begin ,@exprs)))))))

; Our let macro is like the one in arc - just
; a single variable, single expression, and no
; nesting.
; We can define "with" later as a macro that
; expands into individual sublets.
(define-macro let (var value body)
    `((,lambda (,var) ,body) ,value))

; Our pattern matching is powerful enough to define 'if'
; as a macro. In fact, it can even handle
; "multicase" if, by expanding the remaining
; clauses into itself.
(define if (macro
    (lambda (condition true-case false-case)
            ; Base case. Anything not false is considered 'true'
                `((,lambda (#f) ,false-case
                            _   ,true-case) ,condition)
            ; Multiple clauses. Expand into recursive if statements.
            (condition true-case . remainder)
                `(,if ,condition ,true-case
                       (,if ,@remainder)))))

; Now add support for multiple sub-statements in define:
; Whenever we see
; (define x expr1 expr2 ...)
; we'll expand it to
; (raw-define x (begin expr1 expr2 ...))
; Also, whenever we see
; (define (name arg1 arg2) expr1 expr2)
; we'll expand it to 
; (define name (lambda arg1 arg2) (begin expr1 expr2))
; This is the traditional syntax used in SICP et al.
; However, we can't support multiple bodies in our
; lambdas without screwing up our nice 'case lambda'
; syntax which I find more useful than being able to
; define multiple bodies in a lambda. You can aways
; use 'begin' explicitly if need be.
(define define
    (macro
        ; Because define itself mutates the environment,
        ; we have to capture the original 'define' here
        ; before we 'hide' it behind our macro replacement.
        ; Otherwise, we go into an infinite loop when expanding.
        (let raw-define define
            (lambda
                ; Traditional function definition
                ((fn . args) . exprs)
                    `(,raw-define ,fn (,lambda ,args (,begin ,@exprs)))
                (symbol . exprs)
                    `(,raw-define ,symbol (,begin ,@exprs))))))

; Y combinator allows us to write recursive code without
; mutating the environment.
(define Y 
    (lambda (m)
       (let z (lambda (f) (m (lambda (a) ((f f) a))))
	     (z z))))
; We could use the Y combinator here, but because we are defining
; 'length' using a define form, we can just recurse directly
(define (length list)
    (define length-tail
        ; Here, we make use of the "pattern matching" in lambda
        (lambda (so-far ()) so-far
                (so-far (x . y))
                    (length-tail (+ 1 so-far) y)))
    (length-tail 0 list))

(define fold-right
    (lambda (op initial ()) initial
            (op initial (x . y)) (fold-right op (op x initial) y)))

(define (reverse l)
    (define reverse-tail
        (lambda (so-far ()) so-far
                (so-far (x . y))
                    (reverse-tail (cons x so-far) y)))
    (reverse-tail '() l))

; mapcar can be defined in terms of fold
(define (mapcar f list)
    (let combiner
        (lambda (x list)
            (cons (f x) list))
        (reverse (fold-right combiner () list))))

(define (map f . ll)
    (define map-tail
        (lambda
            (f so-far (() . rest))
                so-far
            (f so-far ll)
                (map-tail f (cons (apply f (mapcar car ll)) so-far)
                            (mapcar cdr ll))))
    (reverse (map-tail f '() ll)))

(define-macro loop (var values body)
    `(,mapcar (,lambda (,var) ,body) ,values))

; 'with' macro decomposes recursively into nested 'let' statements
(define with (macro
    (lambda (() body)
                body
            ((var . (expr . bindings)) body)
                `(,let ,var ,expr (,with ,bindings ,body)))))

; let/cc provides "escape" functionality
(define-macro let/cc (var body)
    `(,call/cc (,lambda (,var) ,body)))

; In the common case of only wanting to
; dispatch on the pattern of one variable,
; define a convenient macro to de-nest
; the arguments that would otherwise
; be required in a plain lambda expression
(define-macro match (var . cases)
    (define de-nest
        (lambda
            (()) ()
            ((pattern . (body . remaining)))
                `(,(list pattern) ,body ,@(de-nest remaining))))
    `((,lambda ,@(de-nest cases)) ,var))

(define (not expr)
    (match expr
        #f #t
        _ #f))

(define (make-stack)
    (define contents ())
    (lambda ('push arg)
                (begin
                    (log contents)
                    (set! contents (cons arg contents)))
            ('pop)
                (begin
                    (log contents)
                    (let top (car contents)
                        (begin
                            (set! contents (cdr contents))
                            top)))))

(define (current-continuation) 
  (call/cc
   (lambda (cc)
     (cc cc))))

; fail-stack : list[continuation]
(define fail-stack ())

(define (error msg)
    msg)

(define (fail)
    (match fail-stack
        (back-track-point . rest)
            (begin
                (set! fail-stack rest)
                (back-track-point back-track-point))
        _
            ; We never added support for string literals
            ; to the parser! Use '404' to indicate failure!
            (error 404)))

(define (amb choices)
    (let cc (current-continuation)
        (match choices
            () (fail)
            (choice . remaining-choices)
                (begin
                    (set! choices remaining-choices)
                    (set! fail-stack (cons cc fail-stack))
                    choice))))

(define (assert condition)
    (if (not condition)
        (fail)
        #t))
