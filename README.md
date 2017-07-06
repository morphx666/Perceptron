# Perceptron
Simple Perceptron implementation

![Image of Yaktocat](https://xfx.net/stackoverflow/perceptron/perceptron_01.png)

An implementation of a simple perceptron, based on the tutorials ([1](https://www.youtube.com/watch?v=ntKn5TPHHAk&t=870s) and [2](https://www.youtube.com/watch?v=DGxIcDjPzac&t=1183s)) from [The Coding Train](https://www.youtube.com/user/shiffman) by [Daniel Shiffman](http://shiffman.net/).

### How does it work?

A more complete and detailed answer to this question can be obtained by watching the tutorials linked above.

A line is created inside a cartesian plane, following the `y=mx+b` equation.
Then, a number of points are scattered across the plane, in a random fashion. The perceptron then starts adjusting its weights by asking each point if it appears on one side (black dots) or the other (white dots) from the line.

Using this simple logic, the perceptron can find the original line's parameters in (usually) several thousands of iterations. Of course, the number of points will greatly affect the final accuracy of the "deduced" line.

Please note that some parameters in the implementation have been intentionally altered to enhance the experience of watching the algorithm work, by slowing it down.

### Legend information

The legend on top/left corner displays the following information:

- The weights' values (w1, w2 and w3)
- The user defined line equation (blue)
- The line equation "found" by the algorithm (red)
- The frames/second
- The number of iterations the training algorithm has been executed
- And the runtime

### The dots on the screen

- Each dot is represented by a 16x16 circle
- White dots appear on one side of the line
- Black dots appear on the other side of the line
- Dots with an inner and smaller green circle indicate dots that appear on the same side of both, the pre-defined line (blue) and the line found by the algorithm
- Dots with an inner red circle indicate dots on the wrong side of the algorithm's line


