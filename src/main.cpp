#include "css/css_parser_token_stream.hpp"
#include <iostream>
#include <string_view>

// Helper to convert token type to string for printing
const char* token_type_to_string(css::CSSParserTokenType type) {
    switch (type) {
        case css::CSSParserTokenType::Ident: return "Ident";
        case css::CSSParserTokenType::Function: return "Function";
        case css::CSSParserTokenType::AtKeyword: return "AtKeyword";
        case css::CSSParserTokenType::Hash: return "Hash";
        case css::CSSParserTokenType::String: return "String";
        case css::CSSParserTokenType::BadString: return "BadString";
        case css::CSSParserTokenType::Url: return "Url";
        case css::CSSParserTokenType::BadUrl: return "BadUrl";
        case css::CSSParserTokenType::Delimiter: return "Delimiter";
        case css::CSSParserTokenType::Number: return "Number";
        case css::CSSParserTokenType::Percentage: return "Percentage";
        case css::CSSParserTokenType::Dimension: return "Dimension";
        case css::CSSParserTokenType::Whitespace: return "Whitespace";
        case css::CSSParserTokenType::Cdo: return "CDO";
        case css::CSSParserTokenType::Cdc: return "CDC";
        case css::CSSParserTokenType::Colon: return "Colon";
        case css::CSSParserTokenType::Semicolon: return "Semicolon";
        case css::CSSParserTokenType::Comma: return "Comma";
        case css::CSSParserTokenType::LeftBracket: return "[";
        case css::CSSParserTokenType::RightBracket: return "]";
        case css::CSSParserTokenType::LeftParenthesis: return "(";
        case css::CSSParserTokenType::RightParenthesis: return ")";
        case css::CSSParserTokenType::LeftBrace: return "{";
        case css::CSSParserTokenType::RightBrace: return "}";
        case css::CSSParserTokenType::Comment: return "Comment";
        case css::CSSParserTokenType::Eof: return "EOF";
        default: return "Unknown";
    }
}

int main() {
    std::string_view test_css = R"css(
        body {
            font-size: 16px; /* A comment */
            background-color: #fafafa;
            width: calc(100% - 20px);
        }
    )css";

    css::CSSParserTokenStream stream(test_css);

    std::cout << "Tokenizing CSS: \n" << test_css << "\n\n";

    while (!stream.at_end()) {
        css::CSSParserToken token = stream.consume();
        
        std::cout << "-> " << token_type_to_string(token.type());

        if (token.type() == css::CSSParserTokenType::Ident || token.type() == css::CSSParserTokenType::String || token.type() == css::CSSParserTokenType::Url || token.type() == css::CSSParserTokenType::Function) {
            std::cout << ": " << token.value();
        } else if (token.type() == css::CSSParserTokenType::Number || token.type() == css::CSSParserTokenType::Percentage) {
            std::cout << ": " << token.numeric_value();
        } else if (token.type() == css::CSSParserTokenType::Dimension) {
            std::cout << ": " << token.numeric_value() << token.value();
        } else if (token.type() == css::CSSParserTokenType::Delimiter) {
            std::cout << ": " << token.delimiter();
        } else if (token.type() == css::CSSParserTokenType::Hash) {
            std::cout << ": " << token.value();
        }

        std::cout << std::endl;
    }
    
    std::cout << "-> EOF" << std::endl;

    return 0;
}
