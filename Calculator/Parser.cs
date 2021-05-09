﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ExtensionMethods;
using System.Text;
using System.Threading.Tasks;
using DecimalMath;
using System.Text.RegularExpressions;

namespace Calculator {
    class Parser {
        //parse a string into reverse polish notation
        private readonly Lexer lexer;
        private readonly Dictionary<string, int> precedence = new() {
            ["^"] = 5,
            ["~"] = 4,
            ["*"] = 3,
            ["/"] = 3,
            ["+"] = 2,
            ["-"] = 2,
        };
        private const int right_associative = 0;
        private const int left_associative = 1;
        private readonly Dictionary<string, int> associativity = new() {
            ["^"] = right_associative,
            ["~"] = left_associative,
            ["*"] = left_associative,
            ["/"] = left_associative,
            ["+"] = left_associative,
            ["-"] = left_associative,
        };
        private readonly string[] operators = { "+", "-", "*", "/", "^", "~" };
        //constants/variables
        //<string, string> so unary negate works ~
        private readonly Dictionary<string, string> constants = new() {
            ["e"] = DecimalEx.E.ToString(),
            ["pi"] = DecimalEx.Pi.ToString(),
            ["tau"] = (DecimalEx.Pi * 2).ToString(),
        };

        private readonly int _negate_precedence = -4;
        public int NegatePrecedence {
            get => _negate_precedence;
            init {
                precedence["~"] = value;
                _negate_precedence = value;
            }
        }

        public List<string> Parse() {
            Stack<string> tokens = new();

            while (!lexer.empty) {
                string next = lexer.Next();

                //if (constants["a"] == "~2") {
                //    int breakpoint = 2;
                //}

                //constants and variables
                if (constants.ContainsKey(next)) {
                    next = constants[next];

                    if (tokens.Count > 0 && tokens.Peek().All(char.IsDigit)) {
                        tokens.Push("*");
                    }

                    if (next.Contains("~")) {
                        next = next.Replace("~", "");
                        tokens.Push("~");
                    }
                }

                tokens.Push(next);
            }
            tokens = tokens.Reverse();

            lexer.Restart();
            return shunting_yard(tokens);
        }

        private List<string> shunting_yard(Stack<string> tokens) {
            Stack<string> operator_stack = new();
            List<string> rpn = new();

            while (tokens.Count > 0) {
                string token = tokens.Pop();
                if (decimal.TryParse(token, out _)) {
                    rpn.Add(token);

                } else if (token.All(char.IsLetter)) {
                    //operator
                    operator_stack.Push(token);

                } else if (operators.Contains(token)) {
                    while (operator_stack.Count > 0 && operators.Contains(operator_stack.Peek()) &&
                        (precedence[operator_stack.Peek()] > precedence[token] || (precedence[operator_stack.Peek()] == precedence[token] && associativity[token] == left_associative)) &&
                        operator_stack.Peek() != "(") {
                        rpn.Add(operator_stack.Pop());
                    }

                    operator_stack.Push(token);

                } else switch (token) {
                    case "(":
                        operator_stack.Push(token);
                        break;
                    case ")": {
                        while (operator_stack.Count > 0 && operator_stack.Peek() != "(") {
                            rpn.Add(operator_stack.Pop());
                        }
                        //If the stack runs out without finding a left parenthesis, then there are mismatched parentheses.
                        if (operator_stack.Count == 0) {
                            throw new Exception("Mismatched parenthesis");
                        }

                        if (operator_stack.Count > 0 && operator_stack.Peek() == "(") {
                            operator_stack.Pop();
                        }
                        if (operator_stack.Count > 0 && operator_stack.Peek().All(char.IsLetter)) {
                            rpn.Add(operator_stack.Pop());
                        }

                        break;
                    }
                }
            }

            if (tokens.Count != 0) return rpn;
            while (operator_stack.Count > 0) {
                //If the operator token on the top of the stack is a parenthesis, then there are mismatched parentheses.
                if (operator_stack.Peek() == "(" || operator_stack.Peek() == ")") {
                    throw new Exception("Mismatched Parentheses");
                }

                rpn.Add(operator_stack.Pop());
            }

            return rpn;
        }

        private static string implicit_mult(string equation) {
            //number -> open parenthesis 3(
            equation = Regex.Replace(equation, "([0-9])[(]", "$1*(");
            //close parenthesis -> number )3 || close parenthesis -> . ).01
            equation = Regex.Replace(equation, "[)]([0-9|.])", ")*$1");
            //close parenthesis -> open parenthesis
            equation = Regex.Replace(equation, "[)][(]", ")*(");

            return equation;
        }

        public Parser(string str, Dictionary<string, string> variables) {
            lexer = new Lexer(implicit_mult(str));
            foreach (KeyValuePair<string, string> variable in variables) {
                constants.Add(variable.Key, variable.Value);
            }
        }
    }
}
