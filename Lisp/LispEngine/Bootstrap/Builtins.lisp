(define list (lambda x x))
(define car (lambda ((x . y)) x))
(define cdr (lambda ((x . y)) y))
(define nil? (lambda (()) #t _ #f))
(define pair? (lambda ((_ . _)) #t _ #f))
(define let (macro
	      (lambda (var value body)
		     (list (list lambda (list var) body) value))))
