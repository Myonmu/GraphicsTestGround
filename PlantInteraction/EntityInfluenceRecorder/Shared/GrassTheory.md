# Grass Theory

## Abstraction of Grass

We abstract grass as a mass $m$ attached to a deformable elastic rod with length $l_0$. This elastic rod attempts to restore its original upright configuration when bent by external forces. For simplicity, we consider the rod is no longer deformable and only rotates around its base. This can thus be further simplified as a reversed pendulum, which is now a well-studied problem.

## Static Positions

There are 2 possible static positions: 

- Original position (upright), in this case, the supportive force provided by the rod cancels out the gravitational force.
- An object is pressed against the system and the rod leans against the object.

### Abstraction of The Object 

We assume the object's lower-half contour can be described by a second order polynomial. To parameterize the polynomial, we define $R_0$ the maximum extent of the object, and $H_0$ the effective height of the lower portion. We propose the following expression:
$$y_{object}(x) = \frac{H_0}{R_0^2} * x^2$$

We call $K_{entt} = \frac{H_0}{R_0^2}$ the object's *Entity Constant*.

### Leaning Angle

With the position of the grass and the object, we could calculate the relative distance $d$ between them. When $d < R_0 $, we will be able to observe the second static position. As the rod is non-deformable, and the mass is assumed to have no dimension, we outline two different cases to calculate the leaning angle $\theta$ between the ground and the grass.

- The grass is a tangent of the object's curve (the extremity is not on the curve).
- The grass's extremity is on the curve.

The second case is not practical to calculate and would provide limited realism compared to the first case. We will cover its calculation, but in practice, we simply treat all cases as case 1.

We will treat grass as a line represented by (note that the line passes through $(0,d)$):

$$y_{grass}(x) = k(x - d) $$

#### The Tangent Case

Assume the intersection between the grass segment and the curve is $x_0$. The derivative of the curve at $x_0$ provides the slope of the grass line. Hence:
$$k=y_{object}'(x_0)=2K_{entt}x_0$$
And since the line intersects with the curve at $x_0$, we have:
$$x_0^2*K_{entt}=2K_{entt}x_0(x_0-d)$$
after simplification:
$$x_0 = 2d$$
the line equation becomes:
$$y_{grass}(x)=4*K_{entt}*d(x-d)$$
since $k$ is the tangent of the leaning angle, we have:
$$\theta = arctan(4*K_{entt}*d)$$

#### The Other Case
The intersection is $(x_0,K_{entt}*x_0^2)$ by evaluating the object curve at $x_0$. The displacement relative to $(d,0)$ is $(x_0-d,K_{entt}*x_0^2)$. This vector has length of $l_0$. Then:

$$(x_0-d)^2+K_{entt}^2*x_0^4=l_0^2$$

after expansion:

$$K_{entt}^2*x_0^4+x_0^2-2d*x_0+(d^2-l_0^2)=0$$

This is not practical to solve. 

We do have two special points of interest:

- when $x_0 = d$, the grass is upright but touching the object. Evaluating this condition gives us $d=\sqrt{\frac{l_0}{K_{entt}}}$
- when we are in both case 1 and case 2 (the grass segment is both tangent of the curve and the extremity touches the curve). Evaluating this condition gives us $16*K_{entt}*d^4+d^2-l_0^2 = 0$

But we will leave it here.

