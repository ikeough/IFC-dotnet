grammar STEP;

author
	: value
	;

authorisation
	: value
	;

collection
	: '(' collectionValue (',' collectionValue)* ')'
	;

collectionValue
	: value
	| Id
	;

constructor
	: TypeRef '(' parameter? (',' parameter)* ')' ';'
	;

data
	: instance*
	;

description
	: '(' value ')'
	;

file
	: ISO header data ISO_END
	;

fileDescription
	: FILE_DESCRIPTION '(' description ',' implementation ')' ';'
	;

fileName
	: FILE_NAME '(' name ',' timeStamp ',' author ',' organization ',' preprocessor_version ',' originating_system ',' authorisation ')' ';'
	;

fileSchema
	: FILE_SCHEMA '(' '(' value ')' ')' ';'
	;

header
	: HEADER fileDescription fileName fileSchema END_SEC ';'
	;

implementation
	: value
	;

instance
	: Id '=' constructor ';'
	;

name
	: value
	;

originating_system
	: value
	;

organization
	: value
	;

parameter
	: collection
	| value
	| Undefined
	| StringLiteral
	;

preprocessor_version
	: value
	;

timeStamp
	: value
	;

value
	: '\'' .*? '\''
	;

// Lexer

Digit
	: [0-9]
	;

Digits
	: Digit Digit*
	;

Letter
	: [a-zA-Z]
	;

CapitalLetter
	: [A-Z]
	;

ENDSEC : 'ENDSEC';
FILE_DESCRIPTION : 'FILE_DESCRIPTION';
FILE_NAME : 'FILE_NAME';
FILE_SCHEMA : 'FILE_SCHEMA';
HEADER : 'HEADER';
Id 
	: '#' Digits 
	;

ISO 
	: 'ISO' '-' Digits '-' Digits ';' 
	;

ISO_END 
	: 'END-ISO' '-' Digits '-' Digits ';' 
	;

StringLiteral
	: '"' Letter* '"'
	;

TypeRef
	: CapitalLetter CapitalLetter*
	;
	
Undefined 
	: '$' 
	;

NewlineChar 
	: [\r\n\u000c]+ -> skip ;

WS 
	: [ \t\r\n\u000c]+ -> skip ;

Comments 
	: '/*' .*? '*/' -> skip ;