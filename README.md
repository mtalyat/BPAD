#BPAD

##Introduction

Behavior Pattern Authoring and Detection

BPAD is a querying language, used specifically to find patterns within codes of behaviors. These codes can come from any type of software, so long as they are represented as strings and integers.

This library provides two basic functionalities. The first is parsing Patterns, and the other is evaluating them. To learn more, check out the **Getting Started** section below.

## Examples

If we want a “Ask for information” pattern where a player asks for input, gets input from another player within 5 seconds, and then uses that input within 20 seconds…
`Ask for input -> {5s}Acknowledge input -> {20s}Use input => Ask for information`

If we want a “Panicking” pattern where a player gives a status update or asks for help, 2 or more times in a row within 4 seconds of each other…
`{4s}Ask for help[2+] | {4s} Status update[2+] => Panicking`

If we want a “Panicking” pattern where a player gives a status update, and then asks for help, or vice versa, within 4 seconds of each other…
`(Ask for help | Status update) -> {4s}(Ask for help | Status update) => Panicking`

If we want a “Detailed planning session” where a player gives a goal set and at least one define task, all within 1 minute of planning…
`(Goal set & {5s}Define task[+]) -> {60s}Planning => Detailed planning session`

##Syntax

### Basics
| Syntax | Meaning |
| ----------- | ----------- |
| ( ) | Indicates a group of behavior conditions. Behaves as normal parenthesis within math. |
| [ ] | Indicates flags for a condition (extra information). |
| { } | Indicate timing conditions. |
| \| | Logical or operator. Requires one or more of the two behaviors to occur. The order does not matter. |
| & | Logical and operator. Requires the two behaviors to occur. The order does not matter. |
| -> | Then operator. Used to indicate order of how the behaviors are matched. |
| => | Result operator. Indicates the result if all the conditions on the left are met. |

### Timing
| Syntax | Meaning |
| ----------- | ----------- |
| Ns | The behavior must be within N number of seconds, where N must be to the left of the s, and a positive number. N can be a whole number or a decimal number. |
| N:Ms | The behavior must be between N and M seconds after the previous behavior. |

Examples:
2s
1.25s
90s
3.5:5s

### Flags
| Syntax | Meaning |
| ----------- | ----------- |
| Nx | The behavior with this flag must occur N number of times, where the number of times is to the left of the x. The number of times must be at least 2. |
| N:Mx  or  :Mx  or  N:x | The behavior with this flag must occur between N and M number of times, where N is to the left of the , and M is to the right of it, all to the left of the x. If there is no N, it is assumed to be 0. If there is no M, it is assumed to have no limit (infinity). |
| N+  + | The behavior with this flag must occur N number of times or more, where the number of times is to the left of the +. The number of times must be at least 2, or empty, which defaults the number of times to 1. |
| N-  - | The behavior with this flag must occur N number of times or less, where the number of times is to the left of the -. The number of times must be at least 1, or empty, which defaults the number of times to 1. |
| ! | The behavior with this flag must not occur at all. |
| ? | The behavior with this flag is optional, and may occur once, or not at all. |
| * | The behavior with this flag is optional, and may occur any number of times, including not at all. |
