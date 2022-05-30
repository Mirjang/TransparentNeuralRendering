# TransparentNeuralRendering

Extention of https://github.com/JustusThies/NeuralTexGen

Neural rendering combines classical rendering techniques with learnable elements. It has shown great potential in areas, where the geometry of an object cannot be obtained at a fine enough resolution for the object to be re-rendered using the classical pipeline. Neural rendering has also great success generating images from novel view points.

In this work we apply the deferred neural rendering technique to transparent objects and complex scenes. We use a classical algorithm known as depth peeling. This algorithm renders the scene multiple times and during each iteration stores and removes the nearest surface from the scene, by using a depth mask. For each depth peeling pass, we compute per-pixel UV-coordinates and use them to look up 
a neural texture, as is done in https://niessnerlab.org/projects/thies2019neural.html . We then stack all these layers and pass them through a neural network to obtain a final image. This allows us to see through transparent objects, where the term "transparent" also includes objects with holes, that are covered by a simplified geometry. It also complicates the original deferred neural rendering problem, as our network not only has to learn a rendering function, but also how to perform a blending operation on multiple objects.
