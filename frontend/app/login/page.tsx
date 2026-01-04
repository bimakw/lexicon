'use client';

import { useState } from 'react';
import { useRouter } from 'next/navigation';
import { BookOpen, Mail, Lock, ArrowRight, Sparkles, Github, Chrome, AlertCircle } from 'lucide-react';
import { Button } from '@/components/ui/button';
import { useAuth } from '@/contexts/AuthContext';

export default function LoginPage() {
  const router = useRouter();
  const { login, isAuthenticated } = useAuth();
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  // Redirect if already authenticated
  if (isAuthenticated) {
    router.push('/');
    return null;
  }

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setIsLoading(true);
    setError(null);

    try {
      await login({ usernameOrEmail: email, password });
      router.push('/');
    } catch (err: unknown) {
      const error = err as { response?: { data?: { message?: string } } };
      setError(error.response?.data?.message || 'Login failed. Please check your credentials.');
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="min-h-screen flex">
      {/* Left Side - Branding */}
      <div className="hidden lg:flex lg:w-1/2 relative overflow-hidden">
        {/* Gradient Background */}
        <div className="absolute inset-0 bg-gradient-to-br from-indigo-600 via-purple-600 to-pink-500" />

        {/* Animated Shapes */}
        <div className="absolute inset-0 overflow-hidden">
          <div className="absolute -top-40 -left-40 w-80 h-80 bg-white/10 rounded-full blur-3xl animate-pulse" />
          <div className="absolute top-1/2 -right-20 w-60 h-60 bg-pink-400/20 rounded-full blur-3xl animate-pulse" style={{ animationDelay: '1s' }} />
          <div className="absolute -bottom-20 left-1/3 w-72 h-72 bg-indigo-400/20 rounded-full blur-3xl animate-pulse" style={{ animationDelay: '2s' }} />
        </div>

        {/* Grid Pattern */}
        <div className="absolute inset-0 opacity-10" style={{
          backgroundImage: `linear-gradient(rgba(255,255,255,0.1) 1px, transparent 1px), linear-gradient(90deg, rgba(255,255,255,0.1) 1px, transparent 1px)`,
          backgroundSize: '50px 50px'
        }} />

        {/* Content */}
        <div className="relative z-10 flex flex-col justify-center px-16 text-white">
          {/* Logo */}
          <div className="flex items-center gap-4 mb-12">
            <div className="p-4 rounded-2xl bg-white/10 backdrop-blur-sm border border-white/20">
              <BookOpen className="h-10 w-10" />
            </div>
            <div>
              <h1 className="text-3xl font-bold">Lexicon</h1>
              <p className="text-white/70 flex items-center gap-1">
                <Sparkles className="h-3 w-3" />
                CMS Platform
              </p>
            </div>
          </div>

          {/* Tagline */}
          <h2 className="text-4xl font-bold leading-tight mb-6">
            Manage your content<br />
            <span className="text-white/80">with elegance</span>
          </h2>
          <p className="text-lg text-white/70 max-w-md mb-12">
            A modern, intuitive content management system designed for creators who demand excellence.
          </p>

          {/* Features */}
          <div className="space-y-4">
            {[
              'Beautiful dashboard with real-time analytics',
              'Seamless content creation and management',
              'Powerful collaboration tools',
            ].map((feature, i) => (
              <div key={i} className="flex items-center gap-3">
                <div className="w-6 h-6 rounded-full bg-white/20 flex items-center justify-center">
                  <div className="w-2 h-2 rounded-full bg-white" />
                </div>
                <span className="text-white/90">{feature}</span>
              </div>
            ))}
          </div>
        </div>
      </div>

      {/* Right Side - Login Form */}
      <div className="w-full lg:w-1/2 flex items-center justify-center p-8 bg-gradient-to-br from-slate-50 via-white to-slate-100">
        <div className="w-full max-w-md">
          {/* Mobile Logo */}
          <div className="lg:hidden flex items-center justify-center gap-3 mb-12">
            <div className="p-3 rounded-xl bg-gradient-to-br from-indigo-500 to-purple-600 shadow-lg shadow-indigo-500/30">
              <BookOpen className="h-8 w-8 text-white" />
            </div>
            <span className="text-2xl font-bold text-gradient">Lexicon</span>
          </div>

          {/* Welcome Text */}
          <div className="text-center lg:text-left mb-10">
            <h2 className="text-3xl font-bold text-slate-800 mb-2">Welcome back</h2>
            <p className="text-slate-500">Sign in to your account to continue</p>
          </div>

          {/* Error Message */}
          {error && (
            <div className="mb-6 p-4 bg-red-50 border border-red-200 rounded-xl flex items-center gap-3 text-red-700">
              <AlertCircle className="h-5 w-5 flex-shrink-0" />
              <span className="text-sm">{error}</span>
            </div>
          )}

          {/* Login Form */}
          <form onSubmit={handleSubmit} className="space-y-6">
            {/* Email */}
            <div className="space-y-2">
              <label className="block text-sm font-semibold text-slate-700">Email or Username</label>
              <div className="relative">
                <Mail className="absolute left-4 top-1/2 -translate-y-1/2 h-5 w-5 text-slate-400" />
                <input
                  type="text"
                  value={email}
                  onChange={(e) => setEmail(e.target.value)}
                  placeholder="name@company.com or username"
                  className="w-full pl-12 pr-4 py-3.5 bg-white/70 backdrop-blur-sm border border-slate-200/50 rounded-xl text-slate-800 placeholder:text-slate-400 focus:outline-none focus:ring-2 focus:ring-indigo-500/20 focus:border-indigo-300 transition-all duration-300 shadow-sm hover:shadow-md"
                  required
                />
              </div>
            </div>

            {/* Password */}
            <div className="space-y-2">
              <div className="flex items-center justify-between">
                <label className="block text-sm font-semibold text-slate-700">Password</label>
                <a href="#" className="text-sm text-indigo-600 hover:text-indigo-700 font-medium">
                  Forgot password?
                </a>
              </div>
              <div className="relative">
                <Lock className="absolute left-4 top-1/2 -translate-y-1/2 h-5 w-5 text-slate-400" />
                <input
                  type="password"
                  value={password}
                  onChange={(e) => setPassword(e.target.value)}
                  placeholder="Enter your password"
                  className="w-full pl-12 pr-4 py-3.5 bg-white/70 backdrop-blur-sm border border-slate-200/50 rounded-xl text-slate-800 placeholder:text-slate-400 focus:outline-none focus:ring-2 focus:ring-indigo-500/20 focus:border-indigo-300 transition-all duration-300 shadow-sm hover:shadow-md"
                  required
                />
              </div>
            </div>

            {/* Remember Me */}
            <div className="flex items-center gap-3">
              <input
                type="checkbox"
                id="remember"
                className="w-4 h-4 rounded border-slate-300 text-indigo-600 focus:ring-indigo-500"
              />
              <label htmlFor="remember" className="text-sm text-slate-600">
                Remember me for 30 days
              </label>
            </div>

            {/* Submit Button */}
            <Button
              type="submit"
              disabled={isLoading}
              className="w-full py-4 text-base"
              size="lg"
            >
              {isLoading ? (
                <div className="flex items-center gap-2">
                  <div className="w-5 h-5 border-2 border-white/30 border-t-white rounded-full animate-spin" />
                  Signing in...
                </div>
              ) : (
                <div className="flex items-center gap-2">
                  Sign in
                  <ArrowRight className="h-5 w-5" />
                </div>
              )}
            </Button>
          </form>

          {/* Divider */}
          <div className="relative my-8">
            <div className="absolute inset-0 flex items-center">
              <div className="w-full border-t border-slate-200" />
            </div>
            <div className="relative flex justify-center text-sm">
              <span className="px-4 bg-white text-slate-500">Or continue with</span>
            </div>
          </div>

          {/* Social Login */}
          <div className="grid grid-cols-2 gap-4">
            <button className="flex items-center justify-center gap-2 px-4 py-3 bg-white border border-slate-200 rounded-xl text-slate-700 font-medium hover:bg-slate-50 hover:border-slate-300 transition-all duration-300 shadow-sm hover:shadow-md">
              <Github className="h-5 w-5" />
              GitHub
            </button>
            <button className="flex items-center justify-center gap-2 px-4 py-3 bg-white border border-slate-200 rounded-xl text-slate-700 font-medium hover:bg-slate-50 hover:border-slate-300 transition-all duration-300 shadow-sm hover:shadow-md">
              <Chrome className="h-5 w-5" />
              Google
            </button>
          </div>

          {/* Sign Up Link */}
          <p className="mt-8 text-center text-slate-600">
            Don&apos;t have an account?{' '}
            <a href="/register" className="text-indigo-600 hover:text-indigo-700 font-semibold">
              Sign up for free
            </a>
          </p>
        </div>
      </div>
    </div>
  );
}
